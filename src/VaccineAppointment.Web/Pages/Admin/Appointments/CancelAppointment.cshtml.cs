using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class CancelAppointmentModel : PageModel
    {
        private readonly ILogger<AppointmentDetailsModel> _logger;
        private readonly AppointmentService _service;
        public LocalDate SelectedDate { get; set; }

        public AppointmentAggregate? Slot { get; set; }
        public Appointment? Appointment { get; set; }
        public string? ErrorMessage { get; private set; }

        public CancelAppointmentModel(ILogger<AppointmentDetailsModel> logger, AppointmentService service)
        {
            _logger = logger;
            _service = service;
        }

        private async Task<IActionResult> PageResult(int year, int month, int day, string slotId, string id)
        {
            SelectedDate = new LocalDate(year, month, day);
            Slot = await _service.FindAppointmentSlotByIdAsync(slotId);
            if (Slot == null)
            {
                return RedirectToPage("Index", new { year, month, day });
            }
            Appointment = Slot.FindAppointmentById(id);
            if (Appointment == null)
            {
                return RedirectToPage("SlotDetails", new { year, month, day, id = slotId });
            }
            return Page();
        }

        public async Task<IActionResult> OnGet([FromQuery] int year, [FromQuery] int month, [FromQuery] int day, [FromQuery] string slotId, [FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("Index");
            }
            return await PageResult(year, month, day, slotId, id);
        }

        public async Task<IActionResult> OnPost([FromQuery] int year, [FromQuery] int month, [FromQuery] int day, [FromQuery] string slotId, [FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                return await PageResult(year, month, day, slotId, id);
            }
            var result = await _service.DeleteAppointmentAsync(id);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
                return await PageResult(year, month, day, slotId, id);
            }
            return RedirectToPage("SlotDetails", new { year, month, day, id = slotId });
        }
    }
}
