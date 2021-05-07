using NodaTime;
using System.Collections.Generic;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    public class NavigationViewModel
    {
        public NavigationViewModel(LocalDate selectedDate, string linkPage, params NavigationLink[] links)
        {
            SelectedDate = selectedDate;
            LinkPage = linkPage;
            CommandLinks.AddRange(links);
        }

        public LocalDate SelectedDate { get; set; }
        public string? SlotId { get; set; }
        public bool DayNavigationDisabled { get; set; }
        public string LinkPage { get; set; }
        public string? BackPage { get; set; }

        public List<NavigationLink> CommandLinks { get; } = new List<NavigationLink>();
    }
    public class NavigationLink
    {
        public NavigationLink(string href, string title, string color, bool disabled = false)
        {
            Href = href;
            Title = title;
            Color = color;
            Disabled = disabled;
        }

        public string Href { get; }
        public string Title { get; }
        public string Color { get; }
        public bool Disabled { get; }
    }

}
