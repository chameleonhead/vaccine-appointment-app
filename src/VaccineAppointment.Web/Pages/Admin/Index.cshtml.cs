using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VaccineAppointment.Web.Pages.Admin
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Redirect("/Admin/Appointments");
        }
    }
}
