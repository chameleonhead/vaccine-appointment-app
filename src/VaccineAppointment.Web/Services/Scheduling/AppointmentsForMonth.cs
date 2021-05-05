using NodaTime;
using System.Collections.Generic;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentsForMonth
    {
        public AppointmentsForMonth(YearMonth month, bool prevMonthAvailable, bool nextMonthAvailable, List<AppointmentsForDay> appointments)
        {
            PrevMonthAvailable = prevMonthAvailable;
            NextMonthAvailable = nextMonthAvailable;
            Month = month;
            Appointments = appointments;
        }

        public YearMonth Month { get; set; }
        public bool PrevMonthAvailable { get; set; }
        public bool NextMonthAvailable { get; set; }
        public List<AppointmentsForDay> Appointments { get; }
    }
}
