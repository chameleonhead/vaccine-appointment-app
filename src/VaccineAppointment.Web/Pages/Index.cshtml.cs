using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Threading.Tasks;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages
{

    public class IndexModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;

        public YearMonth Month { get; set; }
        public YearMonth PrevMonth { get; set; }
        public YearMonth NextMonth { get; set; }

        public MonthlyAppointment? Appointments { get; set; }

        public IndexModel(ILogger<IndexModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        private void SetMonth(YearMonth month)
        {
            Month = month;
            PrevMonth = Month.ToDateInterval().Start.PlusDays(-1).ToYearMonth();
            NextMonth = Month.ToDateInterval().End.PlusDays(1).ToYearMonth();
        }

        public async Task OnGet([FromQuery] int? year, [FromQuery] int? month)
        {
            if (year.HasValue && month.HasValue)
            {
                SetMonth(new YearMonth(year.Value, month.Value));
            }
            else
            {
                SetMonth(TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").AtStrictly(LocalDateTime.FromDateTime(DateTime.UtcNow)).Date.ToYearMonth());
            }
            Appointments = await _service.SearchAppointmentsByYearMonthAsync(Month);
        }
    }
}
