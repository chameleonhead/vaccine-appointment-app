using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Mailing;
using VaccineAppointment.Web.Models.Mailing.MessageParams;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Mailing;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentService
    {
        private readonly IAppointmentAggregateRepository _repository;
        private readonly IAppointmentConfigManager _configManager;
        private readonly EmailService _emailService;

        public AppointmentService(IAppointmentAggregateRepository repository, IAppointmentConfigManager configManager, EmailService emailService)
        {
            _repository = repository;
            _configManager = configManager;
            _emailService = emailService;
        }

        public async Task<AppointmentsForMonth> SearchAppointmentsByYearMonthAsync(YearMonth yearMonth)
        {
            var interval = yearMonth.ToDateInterval();
            var aggregates = await _repository.SearchAsync(interval.Start, interval.End);
            var config = await _configManager.GetConfigAsync();

            var response = new AppointmentsForMonth(yearMonth, config);
            foreach (var date in interval)
            {
                response.Appointments.Add(new AppointmentsForDay(date, config, aggregates.Where(a => a.From.Date == date).ToList()));
            }
            return response;
        }

        public async Task<AppointmentsForDay> SearchAppointmentsByDateAsync(LocalDate date)
        {
            var aggregates = await _repository.SearchAsync(date, date);
            var config = await _configManager.GetConfigAsync();
            return new AppointmentsForDay(date, config, aggregates.Where(a => a.From.Date == date).ToList());
        }

        public async Task<AppointmentAggregate?> FindAppointmentSlotByIdAsync(string appointmentSlotId)
        {
            return await _repository.FindBySlotIdAsync(appointmentSlotId);
        }

        public async Task<AppointmentAggregate?> FindAppointmentByIdAsync(string appointmentId)
        {
            return await _repository.FindByAppointmentIdAsync(appointmentId);
        }

        public async Task<OperationResult> CreateMultipleAppointmentSlotsAsync(List<LocalDate> selectedDates, LocalTime from, Period duration, int countOfSlot, int repeatCount)
        {
            var repeatedDuration = Period.FromTicks(duration.ToDuration().ToTimeSpan().Ticks * repeatCount);
            if (repeatedDuration.ToDuration().TotalDays > 1)
            {
                return OperationResult.Fail("1日を超えて予約枠を作成することは出来ません。");
            }

            var aggregatesByDate = new Dictionary<LocalDate, AppointmentsForDay>();
            var aggregatesToCreate = new List<AppointmentAggregate>();
            foreach (var date in selectedDates)
            {
                var df = date.At(from);
                var dt = df + repeatedDuration;
                for (var d = df; d < dt; d += duration)
                {
                    if (!aggregatesByDate.TryGetValue(d.Date, out var aggregates))
                    {
                        aggregates = await SearchAppointmentsByDateAsync(d.Date);
                        aggregatesByDate.Add(d.Date, aggregates);
                    }
                    if (aggregates.AllSlots.Any(s => s.IsOverlap(d, duration)))
                    {
                        return OperationResult.Fail("予約枠が重複しています。");
                    }
                    aggregatesToCreate.Add(new AppointmentAggregate(d, duration, countOfSlot));
                }
            }

            await _repository.AddRangeAsync(aggregatesToCreate);
            return OperationResult.Ok();
        }

        public async Task<OperationResult> CreateAppointmentSlotAsync(LocalDateTime from, Period duration, int countOfSlot)
        {
            var aggregates = await SearchAppointmentsByDateAsync(from.Date);
            if (aggregates.AllSlots.Any(s => s.IsOverlap(from, duration)))
            {
                return OperationResult.Fail("予約枠が重複しています。");
            }
            var aggregate = new AppointmentAggregate(from, duration, countOfSlot);
            await _repository.AddAsync(aggregate);
            return CreateAppointmentSlotResult.Ok(aggregate.Id);
        }

        public async Task<OperationResult> UpdateAppointmentSlotAsync(string id, LocalDateTime from, Period duration, int countOfSlot)
        {
            var aggregate = await FindAppointmentSlotByIdAsync(id);
            if (aggregate == null)
            {
                return OperationResult.Fail("予約枠が存在しません。");
            }
            if (!aggregate.CanUpdate)
            {
                return OperationResult.Fail("予約枠に予約があるため更新できません。");
            }
            var aggregates = await SearchAppointmentsByDateAsync(from.Date);
            if (aggregates.AllSlots.Where(s => s.Id != id).Any(s => s.IsOverlap(from, duration)))
            {
                return OperationResult.Fail("予約枠が重複しています。");
            }
            aggregate.EditSlot(from, duration, countOfSlot);
            await _repository.UpdateAsync(aggregate);
            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteAppointmentSlotAsync(string id)
        {
            var aggregate = await FindAppointmentSlotByIdAsync(id);
            if (aggregate == null)
            {
                return OperationResult.Fail("予約枠が存在しません。");
            }
            if (!aggregate.CanUpdate)
            {
                return OperationResult.Fail("予約枠に予約があるため削除できません。");
            }
            await _repository.RemoveAsync(id);
            return OperationResult.Ok();
        }

        public async Task<OperationResult> CreateAppointmentAsync(string id, string name, string email, string sex, int age)
        {
            var aggregate = await FindAppointmentSlotByIdAsync(id);
            if (aggregate == null)
            {
                return OperationResult.Fail("予約枠が存在しません。");
            }
            if (!aggregate.CanCreateAppointment)
            {
                return OperationResult.Fail("予約の上限に達しました。");
            }
            aggregate.AddAppointment(name, email, sex, age);
            await _repository.UpdateAsync(aggregate);
            return OperationResult.Ok();
        }

        public async Task<OperationResult> MakeAppointmentAsync(string id, string name, string email, string sex, int age)
        {
            var aggregate = await FindAppointmentSlotByIdAsync(id);
            if (aggregate == null)
            {
                return OperationResult.Fail("予約枠が存在しません。");
            }
            if (!aggregate.CanCreateAppointment)
            {
                return OperationResult.Fail("予約の上限に達しました。");
            }
            var appointmentId = aggregate.AddAppointment(name, email, sex, age);
            await _repository.UpdateAsync(aggregate);
            await _emailService.SendMailAsync(new EmailMessageParams()
            {
                TemplateName = "AppointmentAcceptedMessage",
                To = email,
                Params = new AppointmentAcceptedMessageParams(appointmentId, aggregate.From.Date, aggregate.From.TimeOfDay, aggregate.To.TimeOfDay, name)
            });
            return MakeAppointmentResult.Ok(appointmentId);
        }
    }
}