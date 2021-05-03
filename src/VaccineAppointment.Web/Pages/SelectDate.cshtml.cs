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

    public class SelectDateModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;

        public LocalDate Date { get; set; }
        public LocalDate PrevDate { get; set; }
        public LocalDate NextDate { get; set; }

        public DailyAppointment? Appointments { get; set; }

        public SelectDateModel(ILogger<IndexModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        private void SetDate(LocalDate date)
        {
            Date = date;
            PrevDate = Date.PlusDays(-1);
            NextDate = Date.PlusDays(1);
        }

        public async Task OnGet([FromQuery] int? year, [FromQuery] int? month, [FromQuery] int? day)
        {
            if (year.HasValue && month.HasValue && day.HasValue)
            {
                SetDate(new LocalDate(year.Value, month.Value, day.Value));
            }
            else
            {
                SetDate(TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").AtStrictly(LocalDateTime.FromDateTime(DateTime.UtcNow)).Date);
            }
            Appointments = await _service.SearchAppointmentsByDateAsync(Date);
        }
    }
}
