using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VaccineAppointment.Web.Infrastructure;
using VaccineAppointment.Web.Models.Users;
using VaccineAppointment.Web.Services.Users;

namespace VaccineAppointment.Web.Tests.Endpoints
{
    [TestClass]
    public class UserServiceTests
    {
        private VaccineAppointmentContext? db;
        private UserRepository? repository;
        private PasswordHasher? hasher;
        private UserService? sut;

        [TestInitialize]
        public void SetUp()
        {
            db = Utils.CreateInMemoryContext();
            repository = new UserRepository(db);
            hasher = new PasswordHasher("Hash");
            sut = new UserService(repository, hasher);
        }

        [TestMethod]
        public async Task FindByUsernameAsync_should_return_null_given_no_user()
        {
            var user = await sut!.FindByUsernameAsync("unknownuser");
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task FindByUsernameAsync_should_return_user_given_user_exists()
        {
            db!.Users.Add(new User()
            {
                Username = "user",
            });
            await db.SaveChangesAsync();

            var user = await sut!.FindByUsernameAsync("user");
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task ValidateUsernameAndPasswordAsync_should_return_fail_if_user_not_exists()
        {
            db!.Users.Add(new User()
            {
                Username = "user",
            });
            await db.SaveChangesAsync();

            var result = await sut!.ValidateUsernameAndPasswordAsync("admin", "pass");
            Assert.AreEqual(false, result.Succeeded);
            Assert.AreEqual("ユーザー名またはパスワードが違います。", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateUsernameAndPasswordAsync_should_return_fail_given_password_not_match()
        {
            db!.Users.Add(new User()
            {
                Username = "admin",
            });
            await db.SaveChangesAsync();

            var result = await sut!.ValidateUsernameAndPasswordAsync("admin", "pass");
            Assert.AreEqual(false, result.Succeeded);
            Assert.AreEqual("ユーザー名またはパスワードが違います。", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateUsernameAndPasswordAsync_should_return_ok()
        {

            db!.Users.Add(new User()
            {
                Username = "admin",
                Password = hasher!.Hash("pass"),
            });
            await db.SaveChangesAsync();

            var result = (ValidateUsernameAndPasswordResult)await sut!.ValidateUsernameAndPasswordAsync("admin", "pass");
            Assert.AreEqual(true, result.Succeeded);
        }
    }
}
