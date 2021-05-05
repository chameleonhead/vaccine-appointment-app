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

        [TestMethod]
        public async Task FindAppointmentSlotByIdAsync_should_return_slot_given_appointment_exists()
        {
            var slot = new AppointmentSlot()
            {
                From = new LocalDateTime(2021, 5, 5, 10, 0),
                Duration = Period.FromHours(1),
                CountOfSlot = 1,
            };
            db!.Slots.Add(slot);

            var appointment = new Appointment()
            {
                From = new LocalDateTime(2021, 5, 5, 10, 0),
                Duration = Period.FromHours(1),
            };
            db!.Appointments.Add(appointment);
            await db!.SaveChangesAsync();

            var aggregate = await sut!.FindAppointmentSlotByIdAsync(slot.Id);
            Assert.IsNotNull(aggregate);
            Assert.IsTrue(aggregate.Appointments.Any());
        }

        [TestMethod]
        public async Task SearchAppointmentsByDateAsync_should_return_aggregate_given_no_slot_no_config()
        {
            var aggregatesForDay = await sut!.SearchAppointmentsByDateAsync(new LocalDate(2021, 5, 5));
            Assert.AreEqual(new LocalDate(2021, 5, 5), aggregatesForDay.Date);
            Assert.IsFalse(aggregatesForDay.PrevDateAvailable);
            Assert.IsFalse(aggregatesForDay.NextDateAvailable);
            Assert.IsFalse(aggregatesForDay.AvailableSlots.Any());
        }

        [TestMethod]
        public async Task SearchAppointmentsByDateAsync_should_return_aggregate_given_no_slot()
        {
            var config = new AppointmentConfig()
            {
                AvailableIntervalStart = new LocalDate(2021, 5, 5),
                AvailableIntervalEnd = new LocalDate(2021, 5, 5),
            };
            db!.AppointmentConfig.Add(config);
            await db!.SaveChangesAsync();

            var aggregatesForDay = await sut!.SearchAppointmentsByDateAsync(new LocalDate(2021, 5, 5));
            Assert.AreEqual(new LocalDate(2021, 5, 5), aggregatesForDay.Date);
            Assert.IsFalse(aggregatesForDay.PrevDateAvailable);
            Assert.IsFalse(aggregatesForDay.NextDateAvailable);
            Assert.IsFalse(aggregatesForDay.AvailableSlots.Any());
        }

        [TestMethod]
        public async Task SearchAppointmentsByDateAsync_should_return_aggregate_given_no_slot_config_prev_true()
        {
            var config = new AppointmentConfig()
            {
                AvailableIntervalStart = new LocalDate(2021, 5, 4),
                AvailableIntervalEnd = new LocalDate(2021, 5, 5),
            };
            db!.AppointmentConfig.Add(config);
            await db!.SaveChangesAsync();

            var aggregatesForDay = await sut!.SearchAppointmentsByDateAsync(new LocalDate(2021, 5, 5));
            Assert.AreEqual(new LocalDate(2021, 5, 5), aggregatesForDay.Date);
            Assert.IsTrue(aggregatesForDay.PrevDateAvailable);
            Assert.IsFalse(aggregatesForDay.NextDateAvailable);
            Assert.IsFalse(aggregatesForDay.AvailableSlots.Any());
        }

        [TestMethod]
        public async Task SearchAppointmentsByDateAsync_should_return_aggregate_given_no_slot_config_next_true()
        {
            var config = new AppointmentConfig()
            {
                AvailableIntervalStart = new LocalDate(2021, 5, 5),
                AvailableIntervalEnd = new LocalDate(2021, 5, 6),
            };
            db!.AppointmentConfig.Add(config);
            await db!.SaveChangesAsync();

            var aggregatesForDay = await sut!.SearchAppointmentsByDateAsync(new LocalDate(2021, 5, 5));
            Assert.AreEqual(new LocalDate(2021, 5, 5), aggregatesForDay.Date);
            Assert.IsFalse(aggregatesForDay.PrevDateAvailable);
            Assert.IsTrue(aggregatesForDay.NextDateAvailable);
            Assert.IsFalse(aggregatesForDay.AvailableSlots.Any());
        }

        [TestMethod]
        public async Task SearchAppointmentsByDateAsync_should_return_aggregate_given_slot_exists()
        {
            var config = new AppointmentConfig()
            {
                AvailableIntervalStart = new LocalDate(2021, 5, 5),
                AvailableIntervalEnd = new LocalDate(2021, 5, 5),
            };
            db!.AppointmentConfig.Add(config);

            var slot = new AppointmentSlot()
            {
                From = new LocalDateTime(2021, 5, 5, 10, 0),
                Duration = Period.FromHours(1),
                CountOfSlot = 1,
            };
            db!.Slots.Add(slot);
            await db!.SaveChangesAsync();

            var aggregatesForDay = await sut!.SearchAppointmentsByDateAsync(new LocalDate(2021, 5, 5));
            Assert.IsTrue(aggregatesForDay.AvailableSlots.Any());
            Assert.IsFalse(aggregatesForDay.AvailableSlots.First().Appointments.Any());
        }

        [TestMethod]
        public async Task SearchAppointmentsByDateAsync_should_return_aggregate_given_appointment_exists()
        {
            var config = new AppointmentConfig()
            {
                AvailableIntervalStart = new LocalDate(2021, 5, 5),
                AvailableIntervalEnd = new LocalDate(2021, 5, 5),
            };
            db!.AppointmentConfig.Add(config);

            var slot = new AppointmentSlot()
            {
                From = new LocalDateTime(2021, 5, 5, 10, 0),
                Duration = Period.FromHours(1),
                CountOfSlot = 1,
            };
            db!.Slots.Add(slot);

            var appointment = new Appointment()
            {
                From = new LocalDateTime(2021, 5, 5, 10, 0),
                Duration = Period.FromHours(1),
            };
            db!.Appointments.Add(appointment);
            await db!.SaveChangesAsync();

            var aggregatesForDay = await sut!.SearchAppointmentsByDateAsync(new LocalDate(2021, 5, 5));
            Assert.IsTrue(aggregatesForDay.AvailableSlots.First().Appointments.Any());
        }

        [TestMethod]
        public async Task SearchAppointmentsByYearMonthAsync_should_return_aggregate_given_no_slot_no_config()
        {
            var aggregatesForMonth = await sut!.SearchAppointmentsByYearMonthAsync(new YearMonth(2021, 5));
            Assert.AreEqual(new YearMonth(2021, 5), aggregatesForMonth.Month);
            Assert.IsFalse(aggregatesForMonth.PrevMonthAvailable);
            Assert.IsFalse(aggregatesForMonth.NextMonthAvailable);
            Assert.IsTrue(aggregatesForMonth.Appointments.Any());
            foreach (var daily in aggregatesForMonth.Appointments)
            {
                Assert.AreEqual(new YearMonth(2021, 5), daily.Date.ToYearMonth());
            }
        }
    }
}
