using NodaTime;
using System.Collections.Generic;
using System.Linq;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentsForSlot
    {
        public string Id => Slot.Id;
        public LocalDateTime From => Slot.From;
        public Period Duration => Slot.Duration;
        public LocalDateTime To => Slot.To;
        public int CountOfSlot => Slot.CountOfSlot;
        public bool CanDelete => !Appointments.Any();
        public bool CanUpdate => !Appointments.Any();

        public AppointmentSlot Slot { get; set; } = new AppointmentSlot();
        public List<Appointment> Appointments { get; } = new List<Appointment>();
    }
}
