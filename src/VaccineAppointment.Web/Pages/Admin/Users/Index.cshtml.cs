using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Users;
using VaccineAppointment.Web.Services.Users;

namespace VaccineAppointment.Web.Pages.Admin.Users
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserService _service;

        public string? Username { get; set; }

        public List<User> Users { get; set; } = new List<User>();

        public IndexModel(ILogger<IndexModel> logger, UserService service)
        {
            _logger = logger;
            _service = service;
        }

        private async Task<IActionResult> PageResult(string? username)
        {
            Users = await _service.SearchUserAsync(username);
            return Page();
        }

        public Task<IActionResult> OnGet([FromQuery] string? username)
        {
            return PageResult(username);
        }
    }
}
