using NodaTime;
using System;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public class Appointment
    {
        public Appointment()
        {
            Id = Guid.NewGuid().ToString();
            Duration = Period.Zero;
            Name = "Unknown";
            Email = "mail@example.com";
            Sex = "F";
            Age = 0;
        }

        public string Id { get; set; }
        public LocalDateTime From { get; set; }
        public Period Duration { get; set; }
        public LocalDateTime To => From.Plus(Duration);
        public string Name { get; set; }
        public string Email { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
    }
}
