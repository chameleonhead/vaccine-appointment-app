namespace VaccineAppointment.Web.Services.Scheduling
{
    public class MakeAppointmentResult
    {
        public bool Success { get; }

        public string? BookingId { get; }
        public string? ErrorMessage { get; }

        public MakeAppointmentResult(string bookingId, string? errorMessage = null)
        {
            Success = !string.IsNullOrEmpty(bookingId);
            BookingId = bookingId;
            ErrorMessage = errorMessage; 
        }

        public static MakeAppointmentResult Ok(string bookingId)
        {
            return new MakeAppointmentResult(bookingId);
        }
    }
}