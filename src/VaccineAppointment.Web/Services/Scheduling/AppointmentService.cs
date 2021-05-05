using NodaTime;
using NodaTime.Text;
using NodaTime.TimeZones;
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

            var response = new AppointmentsForMonth(yearMonth, config.AvailableIntervalStart.ToYearMonth() <= yearMonth, yearMonth <= config.AvailableIntervalEnd.ToYearMonth());
            foreach (var date in interval)
            {
                response.Appointments.Add(new AppointmentsForDay(date, config.AvailableIntervalStart <= date, date <= config.AvailableIntervalEnd, aggregates.Where(a => a.From.Date == date).ToList()));
            }
            return response;
        }

        public async Task<AppointmentsForDay> SearchAppointmentsByDateAsync(LocalDate date)
        {
            var aggregates = await _repository.SearchAsync(date, date);
            var config = await _configManager.GetConfigAsync();
            return new AppointmentsForDay(date, config.AvailableIntervalStart <= date, date <= config.AvailableIntervalEnd, aggregates.Where(a => a.From.Date == date).ToList());
        }

        public async Task<AppointmentAggregate?> FindAppointmentSlotByIdAsync(string appointmentSlotId)
        {
            return await _repository.FindBySlotIdAsync(appointmentSlotId);
        }

        public async Task<Appointment?> FindAppointmentByIdAsync(string appointmentId)
        {
            await Task.Delay(0);
            var instant = InstantPattern.ExtendedIso.Parse(appointmentId);
            var from = instant.Value.WithOffset(TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").GetUtcOffset(instant.Value)).LocalDateTime;
            var response = new Appointment()
            {
                Id = appointmentId,
                From = from,
                Duration = Period.FromHours(1),
            };
            return response;
        }

        public Task<OperationResult> CreateMultipleAppointmentSlotsAsync(LocalDateTime localDateTime, Period duration, int countOfSlot, int repeatCount)
        {
            return Task.FromResult(OperationResult.Ok());
        }

        public Task<OperationResult> CreateAppointmentSlotAsync(LocalDateTime startTime, Period duration, int countOfSlot)
        {
            return Task.FromResult(OperationResult.Ok());
        }

        public Task<OperationResult> UpdateAppointmentSlotAsync(string id, LocalDateTime startTime, Period duration, int countOfSlot)
        {
            return Task.FromResult(OperationResult.Ok());
        }

        public Task<OperationResult> DeleteAppointmentSlotAsync(string id)
        {
            return Task.FromResult(OperationResult.Ok());
        }

        public Task<MakeAppointmentResult> CreateAppointmentAsync(string id, string name, string email, string sex, int age)
        {
            return Task.FromResult(MakeAppointmentResult.Ok(id));
        }

        public Task<MakeAppointmentResult> MakeAppointmentAsync(string id, string name, string email, string sex, int age)
        {
            return Task.FromResult(MakeAppointmentResult.Ok(id));
        }
    }
}