using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Threading.Tasks;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;

        public LocalDate Today { get; set; }
        public YearMonth Month { get; set; }
        public LocalDate SelectedDate { get; set; }
        public YearMonth PrevMonth { get; set; }
        public YearMonth NextMonth { get; set; }

        public AppointmentsForDay? Appointments { get; set; }

        public IndexModel(ILogger<IndexModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        public async Task OnGet([FromQuery] int? year, [FromQuery] int? month, [FromQuery] int? day)
        {
            Today = TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").AtStrictly(LocalDateTime.FromDateTime(DateTime.UtcNow)).Date;
            if (year.HasValue && month.HasValue)
            {
                SetMonth(new YearMonth(year.Value, month.Value));
            }
            else
            {
                SetMonth(Today.ToYearMonth());
            }
            if (day.HasValue)
            {
                SetSelectedDate(Month.OnDayOfMonth(day.Value));
            }
            else
            {
                SetSelectedDate(Today);
            }
            Appointments = await _service.SearchAppointmentsByDateAsync(SelectedDate);
        }

        private void SetMonth(YearMonth month)
        {
            Month = month;
            PrevMonth = Month.ToDateInterval().Start.PlusDays(-1).ToYearMonth();
            NextMonth = Month.ToDateInterval().End.PlusDays(1).ToYearMonth();
        }

        private void SetSelectedDate(LocalDate date)
        {
            SelectedDate = date;
        }
    }
}
