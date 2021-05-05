using NodaTime;
using System;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public class AppointmentConfig
    {
        public AppointmentConfig()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public LocalDate AvailableIntervalStart { get; set; }
        public LocalDate AvailableIntervalEnd { get; set; }
    }
}
