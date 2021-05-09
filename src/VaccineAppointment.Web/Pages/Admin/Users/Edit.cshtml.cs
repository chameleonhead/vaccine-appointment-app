using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Users;
using VaccineAppointment.Web.Services.Users;

namespace VaccineAppointment.Web.Pages.Admin.Users
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserService _service;

        public User UserDetail { get; set; }

        [BindProperty]
        [Required]
        public string? Name { get; set; }
        [BindProperty]
        public string? Password { get; set; }
        [BindProperty]
        [Required]
        public string? Role { get; set; }

        public EditModel(ILogger<IndexModel> logger, UserService service)
        {
            _logger = logger;
            _service = service;
            UserDetail = new User();
        }

        private async Task<IActionResult> ResultPage(string id)
        {
            var user = await _service.FirstByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            UserDetail = user;
            return Page();
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actionResult = await ResultPage(id);

            Name = UserDetail.Name;
            Role = UserDetail.Role;

            return actionResult;
        }

        public async Task<IActionResult> OnPostAsync([FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                return await ResultPage(id);
            }

            var result = await _service.UpdateAsync(id, Password, Role!, Name!);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                return await ResultPage(id);
            }
            return RedirectToPage("./Index");
        }
    }
}
