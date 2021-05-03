using NodaTime;
using System.Collections.Generic;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class MonthlyAppointment
    {
        public MonthlyAppointment(YearMonth month, bool prevMonthAvailable, bool nextMonthAvailable, List<DailyAppointment> appointments)
        {
            PrevMonthAvailable = prevMonthAvailable;
            NextMonthAvailable = nextMonthAvailable;
            Month = month;
            Appointments = appointments;
        }

        public YearMonth Month { get; set; }
        public bool PrevMonthAvailable { get; set; }
        public bool NextMonthAvailable { get; set; }
        public List<DailyAppointment> Appointments { get; }
    }
}
