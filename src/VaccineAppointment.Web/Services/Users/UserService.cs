using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Users;

namespace VaccineAppointment.Web.Services.Users
{
    public class UserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository repository, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _repository.FindByUsernameAsync(username);
        }

        public async Task<OperationResult> ChangePasswordAsync(string username, string newPassword)
        {
            var user = await FindByUsernameAsync(username);
            if (user is null)
            {
                return OperationResult.Fail("ユーザーが存在しません。");
            }
            user.ChangePassword(_passwordHasher.Hash(newPassword));
            await _repository.UpdateAsync(user);
            return OperationResult.Ok();
        }

        public async Task<OperationResult> ValidateUsernameAndPasswordAsync(string username, string password)
        {

            var user = await FindByUsernameAsync(username);
            if (user is null)
            {
                return OperationResult.Fail("ユーザー名またはパスワードが違います。");
            }
            if (user.Password != _passwordHasher.Hash(password))
            {
                return OperationResult.Fail("ユーザー名またはパスワードが違います。");
            }
            return ValidateUsernameAndPasswordResult.Ok(user);
        }
    }
}
