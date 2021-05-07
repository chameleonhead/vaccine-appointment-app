using NodaTime;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public class AppointmentConfig
    {
        public LocalDate? AvailableIntervalStart { get; set; }
        public LocalDate? AvailableIntervalEnd { get; set; }
    }
}
