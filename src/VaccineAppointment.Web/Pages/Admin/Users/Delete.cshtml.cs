using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Users;
using VaccineAppointment.Web.Services.Users;

namespace VaccineAppointment.Web.Pages.Admin.Users
{
    [Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserService _service;

        public User UserDetail { get; set; }

        public DeleteModel(ILogger<IndexModel> logger, UserService service)
        {
            _logger = logger;
            _service = service;
            UserDetail = new User();
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserDetail = await _service.FirstByIdAsync(id);
            if (UserDetail == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromQuery] string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _service.DeleteUserAsync(id);
            return RedirectToPage("./Index");
        }
    }
}
