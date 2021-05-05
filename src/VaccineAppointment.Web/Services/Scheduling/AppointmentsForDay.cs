using NodaTime;
using System.Collections.Generic;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentsForDay
    {
        public AppointmentsForDay(LocalDate date, bool prevDateAvailable, bool nextDateAvailable, List<AppointmentAggregate> slots)
        {
            PrevDateAvailable = prevDateAvailable;
            NextDateAvailable = nextDateAvailable;
            Date = date;
            AvailableSlots = slots;
        }

        public LocalDate Date { get; }
        public bool PrevDateAvailable { get; set; }
        public bool NextDateAvailable { get; set; }
        public List<AppointmentAggregate> AvailableSlots { get; }
    }
}
