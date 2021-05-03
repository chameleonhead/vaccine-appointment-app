using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;
using VaccineAppointment.Web.Authentication;
using VaccineAppointment.Web.Services.Users;

namespace VaccineAppointment.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserService _service;
        private readonly PasswordHasher _passwordHasher;

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

        public LoginModel(UserService service, PasswordHasher passwordHasher)
        {
            _service = service;
            _passwordHasher = passwordHasher;
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

            var user = await _service.FindByUsernameAsync(Username!);
            if (user is null)
            {
                ErrorMessage = "ユーザー名またはパスワードが違います。";
                return Page();
            }

            if (user == null || user.Password != _passwordHasher.Hash(Password!))
            {
                ErrorMessage = "ユーザー名またはパスワードが違います。";
                return Page();
            }

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
