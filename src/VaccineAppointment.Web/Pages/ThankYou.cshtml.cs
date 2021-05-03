using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages
{

    public class ThankYouModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;

        public Appointment? Booking { get; set; }


        public ThankYouModel(ILogger<IndexModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        public async Task OnGet([FromQuery] string id)
        {
            Booking = await _service.FindBookByIdAsync(id);
        }
    }
}
