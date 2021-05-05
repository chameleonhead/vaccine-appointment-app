using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VaccineAppointment.Web.Infrastructure;
using VaccineAppointment.Web.Services.Users;

namespace VaccineAppointment.Web.Tests.Endpoints
{
    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public async Task FindByUsernameAsync_should_return_null_given_no_user()
        {
            var db = Utils.CreateInMemoryContext();
            var repository = new UserRepository(db);
            var hasher = new PasswordHasher("Hash");
            var service = new UserService(repository, hasher);
            var user = await service.FindByUsernameAsync("unknownuser");
            Assert.IsNull(user);
        }
    }
}
