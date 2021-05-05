using NodaTime;

namespace VaccineAppointment.Web.Pages
{
    public class CalendarViewModel
    {
        public YearMonth Month { get; set; }
        public LocalDate? SelectedDate { get; set; }
        public string? LinkPage { get; set; }
    }
}
