using System.Threading.Tasks;
using VaccineAppointment.Web.Authentication;
using VaccineAppointment.Web.Models.Users;

namespace VaccineAppointment.Web.Services.Users
{
    public class UserService
    {
        private readonly PasswordHasher _passwordHasher;

        public UserService(PasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public Task<User?> FindByUsernameAsync(string username)
        {
            if (username.ToLowerInvariant() == "admin")
            {
                return Task.FromResult((User?)new User()
                {
                    Username = "admin",
                    Password = _passwordHasher.Hash("P@ssw0rd"),
                    Role = "Administrator",
                });

            }
            return Task.FromResult((User?)null);
        }
    }
}
