using NodaTime;
using System.Linq;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentService
    {
        private readonly IAppointmentAggregateRepository _repository;
        private readonly IAppointmentConfigManager _configManager;

        public AppointmentService(IAppointmentAggregateRepository repository, IAppointmentConfigManager configManager)
        {
            _repository = repository;
            _configManager = configManager;
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

        public Task<OperationResult> CreateMultipleAppointmentSlotsAsync(LocalDateTime localDateTime, Period duration, int countOfSlot, int repeatCount)
        {
            return Task.FromResult(OperationResult.Ok());
        }

        public async Task<OperationResult> CreateAppointmentSlotAsync(LocalDateTime startTime, Period duration, int countOfSlot)
        {
            var aggregates = await SearchAppointmentsByDateAsync(startTime.Date);
            if (aggregates.AvailableSlots.Any(s => s.IsOverlap(startTime, duration)))
            {
                return OperationResult.Fail("予約枠が重複しています。");
            }
            var aggregate = new AppointmentAggregate(startTime, duration, countOfSlot);
            await _repository.AddAsync(aggregate);
            return CreateAppointmentSlotResult.Ok(aggregate.Id);
        }

        public async Task<OperationResult> UpdateAppointmentSlotAsync(string id, LocalDateTime startTime, Period duration, int countOfSlot)
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
            var aggregates = await SearchAppointmentsByDateAsync(startTime.Date);
            if (aggregates.AvailableSlots.Where(s => s.Id != id).Any(s => s.IsOverlap(startTime, duration)))
            {
                return OperationResult.Fail("予約枠が重複しています。");
            }
            aggregate.EditSlot(startTime, duration, countOfSlot);
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
            var appointmentId = aggregate.AddAppointment(name, email, sex, age);
            await _repository.UpdateAsync(aggregate);
            return MakeAppointmentResult.Ok(appointmentId);
        }
    }
}