namespace VaccineAppointment.Web.Models.Scheduling
{
    public class Booking
    {
        public string? Id { get; set; }
        public AppointmentSlot? Slot { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Sex { get; set; }
        public int Age { get; set; }
    }
}
