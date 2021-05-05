using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;
using VaccineAppointment.Web.Services.Users;

namespace VaccineAppointment.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserService _service;

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

        public LoginModel(UserService service)
        {
            _service = service;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "入力に誤りがあります。";
                return Page();
            }

            var result = await _service.ValidateUsernameAndPasswordAsync(Username!, Password!);
            if (!result.Succeeded || !(result is ValidateUsernameAndPasswordResult validationResult))
            {
                ErrorMessage = "ユーザー名またはパスワードが違います。";
                return Page();
            }
            var user = validationResult.User;

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            }, CookieAuthenticationDefaults.AuthenticationScheme));

            await HttpContext.SignInAsync(principal);
            return Redirect(returnUrl ?? "/Admin");
        }
    }
}
