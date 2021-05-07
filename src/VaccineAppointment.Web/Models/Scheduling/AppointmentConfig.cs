using NodaTime;
using System;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public class AppointmentConfig
    {
        public AppointmentConfig()
        {
            Id = Guid.Empty.ToString();
        }

        public string Id { get; set; }
        public LocalDate? AvailableIntervalStart { get; set; }
        public LocalDate? AvailableIntervalEnd { get; set; }
    }
}
