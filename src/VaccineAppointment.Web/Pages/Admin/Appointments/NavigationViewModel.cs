using NodaTime;

namespace VaccineAppointment.Web.Pages.Appointments
{
    public class NavigationViewModel
    {
        public NavigationViewModel(LocalDate selectedDate, string linkPage)
        {
            SelectedDate = selectedDate;
            LinkPage = linkPage;
        }

        public LocalDate SelectedDate { get; set; }
        public string? SlotId { get; set; }
        public bool DayNavigationDisabled { get; set; }
        public string LinkPage { get; set; }
        public string? BackPage { get; set; }
    }
}
