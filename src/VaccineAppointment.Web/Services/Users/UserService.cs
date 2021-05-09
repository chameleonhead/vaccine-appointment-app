using System;
using System.Collections.Generic;
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

        public Task<List<User>> SearchUserAsync(string? username)
        {
            return _repository.SearchForAsync(username);
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _repository.FindByUsernameAsync(username);
        }

        public async Task<User> FirstByIdAsync(string id)
        {
            return await _repository.FindByIdAsync(id);
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

        public async Task<OperationResult> DeleteUserAsync(string id)
        {
            var user = await _repository.FindByIdAsync(id);
            if (user is null)
            {
                return OperationResult.Fail("ユーザーが見つかりません。");
            }
            if (user.Username == "admin")
            {
                return OperationResult.Fail("管理者は削除できません。");
            }
            await _repository.RemoveAsync(id);
            return OperationResult.Ok();
        }

        public async Task<OperationResult> CreateAsync(string username, string password, string role, string name)
        {
            await _repository.AddAsync(new User(username, password, role, name));
            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateAsync(string id, string? password, string role, string name)
        {
            var user = await _repository.FindByIdAsync(id);
            if (user == null)
            {
                return OperationResult.Fail("ユーザーが見つかりません。");
            }
            if (user.Username == "admin")
            {
                return OperationResult.Fail("管理者は変更できません。パスワードの変更はパスワード変更画面より実施してください。");
            }

            user.ChangeName(name);
            if (!string.IsNullOrEmpty(password))
            {
                user.ChangePassword(_passwordHasher.Hash(password));
            }
            user.ChangeRole(role);
            await _repository.UpdateAsync(user);
            return OperationResult.Ok();
        }
    }
}
