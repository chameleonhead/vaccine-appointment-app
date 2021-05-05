using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Users
{
    public interface IUserRepository
    {
        Task<User> FindByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task UpdateAsync(User user);
    }
}
