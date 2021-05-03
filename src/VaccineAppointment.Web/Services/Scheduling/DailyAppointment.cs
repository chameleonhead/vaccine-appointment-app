using NodaTime;
using System.Collections.Generic;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class DailyAppointment
    {
        public DailyAppointment(LocalDate date, bool prevDateAvailable, bool nextDateAvailable, List<AppointmentSlot> slots)
        {
            PrevDateAvailable = prevDateAvailable;
            NextDateAvailable = nextDateAvailable;
            Date = date;
            AvailableSlots = slots;
        }

        public LocalDate Date { get; }
        public bool PrevDateAvailable { get; set; }
        public bool NextDateAvailable { get; set; }
        public List<AppointmentSlot> AvailableSlots { get; }
    }
}
