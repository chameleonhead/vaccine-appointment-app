using System;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public class Appointment
    {
        public Appointment()
        {
            Id = Guid.NewGuid().ToString();
            Slot = new AppointmentSlot();
            Name = "Unknown";
            Email = "mail@example.com";
            Sex = "F";
            Age = 0;
        }

        public string Id { get; set; }
        public AppointmentSlot Slot { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
    }
}
