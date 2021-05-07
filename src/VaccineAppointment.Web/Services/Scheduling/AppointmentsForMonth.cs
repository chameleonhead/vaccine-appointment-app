using NodaTime;
using System.Collections.Generic;
using System.Linq;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentsForMonth
    {
        public AppointmentsForMonth(YearMonth month, AppointmentConfig config)
        {
            PrevMonthAvailable = config.AvailableIntervalStart == null || config.AvailableIntervalStart.Value.ToYearMonth() < month;
            NextMonthAvailable = config.AvailableIntervalEnd == null || month < config.AvailableIntervalEnd.Value.ToYearMonth();
            Month = month;
        }

        public bool IsAvailable(LocalDate date)
        {
            if (date.ToYearMonth() != Month)
            {
                return false;
            }
            return Appointments.First(a => a.Date == date).AvailableSlots.Any(s => s.RemainingSlots > 0);
        }

        public YearMonth Month { get; set; }
        public bool PrevMonthAvailable { get; set; }
        public bool NextMonthAvailable { get; set; }
        public List<AppointmentsForDay> Appointments { get; } = new();
    }
}
