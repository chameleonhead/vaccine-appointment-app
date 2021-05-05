using NodaTime;

namespace VaccineAppointment.Web.Pages.Appointments
{
    public class NavigationViewModel
    {
        public NavigationViewModel(LocalDate selectedDate, string linkPage, string? backPage = null)
        {

        }

        public YearMonth Month { get; set; }
        public LocalDate SelectedDate { get; set; }
        public string? LinkPage { get; set; }
        public string? BackPage { get; set; }
    }
}
