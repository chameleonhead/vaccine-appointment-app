using NodaTime;
using System;

namespace VaccineAppointment.Web.Pages
{
    public class CalendarViewModel
    {
        public CalendarViewModel(YearMonth month, string? linkPage = null, Func<LocalDate, bool>? isDisable = null)
        {
            Month = month;
            LinkPage = linkPage;
            IsDisable = isDisable ?? (d => false);
        }

        public CalendarViewModel(LocalDate selectedDate, string? linkPage = null, Func<LocalDate, bool>? isDisable = null)
        {
            Month = selectedDate.ToYearMonth();
            SelectedDate = selectedDate;
            LinkPage = linkPage;
            IsDisable = isDisable ?? (d => false);
        }

        public YearMonth Month { get; set; }
        public LocalDate? SelectedDate { get; set; }
        public string? LinkPage { get; set; }
        public Func<LocalDate, bool> IsDisable { get; }
    }
}
