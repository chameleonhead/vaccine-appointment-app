using NodaTime;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public class AppointmentSlot
    {
        public string? Id { get; set; }
        public LocalDateTime From { get; set; }
        public Period? Duration { get; set; }
        public LocalDateTime To => From.Plus(Duration!);
        public int CountOfSlot { get; internal set; }
    }
}
