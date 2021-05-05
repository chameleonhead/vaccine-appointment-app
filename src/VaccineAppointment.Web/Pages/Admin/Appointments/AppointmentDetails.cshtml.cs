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
    public class AppointmentDetailsModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;
        public LocalDate SelectedDate { get; set; }

        public AppointmentsForSlot? Slot { get; set; }
        public Appointment? Appointment { get; set; }

        public AppointmentDetailsModel(ILogger<IndexModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        private async Task<IActionResult> PageResult(int year, int month, int day, string slotId, string id)
        {
            SelectedDate = new LocalDate(year, month, day);
            Slot = await _service.FindAppointmentSlotByIdAsync(slotId);
            if (Slot == null)
            {
                return RedirectToPage("Index", new { year, month, day });
            }
            Appointment = await _service.FindAppointmentByIdAsync(id);
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
    }
}
