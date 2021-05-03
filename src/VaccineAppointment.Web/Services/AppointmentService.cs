using NodaTime;
using NodaTime.Text;
using NodaTime.TimeZones;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Services
{
    public class AppointmentService
    {
        public async Task<MonthlyAppointment> SearchAppointmentsByYearMonthAsync(YearMonth yearMonth)
        {
            var response = new MonthlyAppointment(yearMonth, new YearMonth(2020, 1) < yearMonth, yearMonth < new YearMonth(2021, 12), new List<DailyAppointment>());
            foreach (var date in yearMonth.ToDateInterval())
            {
                response.Appointments.Add(await SearchAppointmentsByDateAsync(date));
            }
            return response;
        }

        public Task<DailyAppointment> SearchAppointmentsByDateAsync(LocalDate date)
        {
            var response = new DailyAppointment(date, new LocalDate(2020, 1, 1) < date, date < new LocalDate(2021, 12, 31), new List<AppointmentSlot>());
            if (date.DayOfWeek != IsoDayOfWeek.Sunday)
            {
                response.AvailableSlots.Add(new AppointmentSlot()
                {
                    Id = InstantPattern.ExtendedIso.Format(TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").AtStrictly(date.At(new LocalTime(10, 0))).ToInstant()),
                    From = date.At(new LocalTime(9, 0)),
                    Duration = Period.FromHours(1),
                    CountOfSlot = 10,
                });
                response.AvailableSlots.Add(new AppointmentSlot()
                {
                    Id = InstantPattern.ExtendedIso.Format(TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").AtStrictly(date.At(new LocalTime(10, 0))).ToInstant()),
                    From = date.At(new LocalTime(10, 0)),
                    Duration = Period.FromHours(1),
                    CountOfSlot = 10,
                });
            }
            return Task.FromResult(response);
        }

        public Task<AppointmentSlot?> FindAppointmentByIdAsync(string appointmentId)
        {
            var instant = InstantPattern.ExtendedIso.Parse(appointmentId);
            var response = new AppointmentSlot()
            {
                Id = appointmentId,
                From = instant.Value.WithOffset(TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").GetUtcOffset(instant.Value)).LocalDateTime,
                Duration = Period.FromHours(1),
                CountOfSlot = 10,
            };
            return Task.FromResult((AppointmentSlot?)response);
        }

        public async Task<Appointment?> FindBookByIdAsync(string bookingId)
        {
            var response = new Appointment()
            {
                Id = bookingId,
                Slot = await FindAppointmentByIdAsync(bookingId),
            };
            return (Appointment?)response;
        }

        public Task<MakeAppointmentResult> MakeAppointmentAsync(string id, string name, string email, string sex, int age)
        {
            return Task.FromResult(MakeAppointmentResult.Ok(id));
        }
    }
}