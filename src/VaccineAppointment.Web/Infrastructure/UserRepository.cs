using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Users;

namespace VaccineAppointment.Web.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly VaccineAppointmentContext db;

        public UserRepository(VaccineAppointmentContext db)
        {
            this.db = db;
        }

        public Task<User> FindByUsernameAsync(string username)
        {
            return db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddUserAsync(User user)
        {
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }
    }
}
