namespace VaccineAppointment.Web.Services
{
    public class BookingResult
    {
        public bool Success { get; }

        public string? BookingId { get; }
        public string? ErrorMessage { get; }

        public BookingResult(string bookingId, string? errorMessage = null)
        {
            Success = !string.IsNullOrEmpty(bookingId);
            BookingId = bookingId;
            ErrorMessage = errorMessage; 
        }

        public static BookingResult Ok(string bookingId)
        {
            return new BookingResult(bookingId);
        }
    }
}