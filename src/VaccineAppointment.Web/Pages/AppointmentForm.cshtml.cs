﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages
{

    public class AppointmentFormModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;

        public AppointmentSlot? AppointmentSlot { get; set; }

        [BindProperty]
        public string? Name { get; set; }
        [BindProperty]
        public string? Email { get; set; }
        [BindProperty]
        public string? Sex { get; set; }
        [BindProperty]
        public int? Age { get; set; }

        public AppointmentFormModel(ILogger<IndexModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        public async Task OnGet([FromQuery] string id)
        {
            AppointmentSlot = await _service.FindAppointmentByIdAsync(id);
        }

        public async Task<IActionResult> OnPost([FromQuery] string id)
        {
            var result = await _service.MakeAppointmentAsync(id, Name!, Email!, Sex!, Age!.Value);
            return RedirectToPage("ThankYou", new { Id = result.BookingId });
        }
    }
}