using NodaTime;
using NodaTime.Text;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentService
    {
        public async Task<AppointmentsForMonth> SearchAppointmentsByYearMonthAsync(YearMonth yearMonth)
        {
            var response = new AppointmentsForMonth(yearMonth, new YearMonth(2020, 1) < yearMonth, yearMonth < new YearMonth(2021, 12), new List<AppointmentsForDay>());
            foreach (var date in yearMonth.ToDateInterval())
            {
                response.Appointments.Add(await SearchAppointmentsByDateAsync(date));
            }
            return response;
        }

        public async Task<AppointmentsForDay> SearchAppointmentsByDateAsync(LocalDate date)
        {
            var response = new AppointmentsForDay(date, new LocalDate(2020, 1, 1) < date, date < new LocalDate(2021, 12, 31), new List<AppointmentAggregate>());
            if (date.DayOfWeek != IsoDayOfWeek.Sunday)
            {
                Func<LocalDateTime, string> patternFactory = dateTime => InstantPattern.ExtendedIso.Format(TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").AtStrictly(dateTime).ToInstant());
                response.AvailableSlots.Add((await FindAppointmentSlotByIdAsync(patternFactory(date.At(new LocalTime(9, 0)))))!);
                response.AvailableSlots.Add((await FindAppointmentSlotByIdAsync(patternFactory(date.At(new LocalTime(10, 0)))))!);
            }
            return response;
        }

        public async Task<AppointmentAggregate> FindAppointmentSlotByIdAsync(string appointmentSlotId)
        {
            var instant = InstantPattern.ExtendedIso.Parse(appointmentSlotId);
            var response = new AppointmentAggregate(new AppointmentSlot()
            {
                Id = appointmentSlotId,
                From = instant.Value.WithOffset(TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").GetUtcOffset(instant.Value)).LocalDateTime,
                Duration = Period.FromHours(1),
                CountOfSlot = 10,
            });
            if (response.From.Hour == 10 && response.From.Minute == 0)
            {
                response.Appointments.Add((await FindAppointmentByIdAsync(appointmentSlotId))!);
            }
            return response;
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