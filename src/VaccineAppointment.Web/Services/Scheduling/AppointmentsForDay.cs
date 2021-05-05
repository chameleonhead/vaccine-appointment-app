using NodaTime;
using System.Collections.Generic;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentsForDay
    {
        public AppointmentsForDay(LocalDate date, bool prevDateAvailable, bool nextDateAvailable, List<AppointmentsForSlot> slots)
        {
            PrevDateAvailable = prevDateAvailable;
            NextDateAvailable = nextDateAvailable;
            Date = date;
            AvailableSlots = slots;
        }

        public LocalDate Date { get; }
        public bool PrevDateAvailable { get; set; }
        public bool NextDateAvailable { get; set; }
        public List<AppointmentsForSlot> AvailableSlots { get; }
    }
}
