using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using System.Linq;
using System.Threading.Tasks;
using VaccineAppointment.Web.Infrastructure;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Tests.Endpoints
{
    [TestClass]
    public class AppointmentServiceTests
    {
        private VaccineAppointmentContext? db;
        private AppointmentAggregateRepository? repository;
        private AppointmentConfigManager? configManager;
        private AppointmentService? sut;

        [TestInitialize]
        public void SetUp()
        {
            db = Utils.CreateInMemoryContext();
            repository = new AppointmentAggregateRepository(db!);
            configManager = new AppointmentConfigManager(db!);
            sut = new AppointmentService(repository, configManager);
        }

        [TestMethod]
        public async Task FindAppointmentSlotByIdAsync_should_return_null_given_no_slots()
        {
            var user = await sut!.FindAppointmentSlotByIdAsync("unknownid");
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task FindAppointmentSlotByIdAsync_should_return_slot()
        {
            var slot = new AppointmentSlot()
            {
                From = new LocalDateTime(2021, 5, 5, 10, 0),
                Duration = Period.FromHours(1),
                CountOfSlot = 1,
            };
            db!.Slots.Add(slot);
            await db!.SaveChangesAsync();

            var aggregate = await sut!.FindAppointmentSlotByIdAsync(slot.Id);
            Assert.IsNotNull(aggregate);
            Assert.IsFalse(aggregate.Appointments.Any());
        }
    }
}
