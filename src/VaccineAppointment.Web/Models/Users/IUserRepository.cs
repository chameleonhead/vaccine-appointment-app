using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Users
{
    public interface IUserRepository
    {
        Task<List<User>> SearchForAsync(string? username);
        Task<User> FindByIdAsync(string id);
        Task<User> FindByUsernameAsync(string username);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task RemoveAsync(string id);
    }
}
