using NodaTime;
using System.Collections.Generic;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentsForMonth
    {
        public AppointmentsForMonth(YearMonth month, bool prevMonthAvailable, bool nextMonthAvailable)
        {
            PrevMonthAvailable = prevMonthAvailable;
            NextMonthAvailable = nextMonthAvailable;
            Month = month;
        }

        public YearMonth Month { get; set; }
        public bool PrevMonthAvailable { get; set; }
        public bool NextMonthAvailable { get; set; }
        public List<AppointmentsForDay> Appointments { get; } = new();
    }
}
