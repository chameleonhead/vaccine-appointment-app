using NodaTime;
using System;
using System.Collections.Generic;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public class AppointmentSlot
    {
        public AppointmentSlot()
        {
            Id = Guid.NewGuid().ToString();
            Duration = Period.Zero;
        }

        public string Id { get; set; }
        public LocalDateTime From { get; set; }
        public Period Duration { get; set; }
        public LocalDateTime To => From.Plus(Duration);
        public int CountOfSlot { get; set; }

        public List<Appointment> Appointments { get; } = new List<Appointment>();
    }
}
