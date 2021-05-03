using NodaTime;
using System.Collections.Generic;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Services
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
