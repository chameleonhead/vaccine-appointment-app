using NodaTime;

namespace VaccineAppointment.Web.Pages
{
    public class CalendarViewModel
    {
        public CalendarViewModel(YearMonth month, string? linkPage = null)
        {
            Month = month;
            LinkPage = linkPage;
        }

        public CalendarViewModel(LocalDate selectedDate, string? linkPage = null)
        {
            Month = selectedDate.ToYearMonth();
            SelectedDate = selectedDate;
            LinkPage = linkPage;
        }

        public YearMonth Month { get; set; }
        public LocalDate? SelectedDate { get; set; }
        public string? LinkPage { get; set; }
    }
}
