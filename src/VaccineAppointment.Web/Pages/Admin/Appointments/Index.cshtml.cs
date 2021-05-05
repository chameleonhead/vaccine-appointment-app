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

        public LocalDate SelectedDate { get; set; }

        public AppointmentsForDay? Appointments { get; set; }

        public IndexModel(ILogger<IndexModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        private async Task<IActionResult> PageResult(int year, int month, int day)
        {
            SelectedDate = new LocalDate(year, month, day);
            Appointments = await _service.SearchAppointmentsByDateAsync(SelectedDate);
            return Page();
        }

        public Task<IActionResult> OnGet([FromQuery] int? year, [FromQuery] int? month, [FromQuery] int? day)
        {
            if (year.HasValue && month.HasValue && day.HasValue)
            {
                return PageResult(year.Value, month.Value, day.Value);
            }

            var today = TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").AtStrictly(LocalDateTime.FromDateTime(DateTime.UtcNow)).Date;
            return PageResult(today.Year, today.Month, today.Day);
        }
    }
}
