using Microsoft.EntityFrameworkCore;
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
                Assert.IsFalse(daily.PrevDateAvailable);
                Assert.IsFalse(daily.NextDateAvailable);
            }
        }

        [TestMethod]
        public async Task SearchAppointmentsByYearMonthAsync_should_return_aggregate_given_no_slots()
        {
            var config = new AppointmentConfig()
            {
                AvailableIntervalStart = new LocalDate(2021, 5, 5),
                AvailableIntervalEnd = new LocalDate(2021, 5, 5),
            };
            db!.AppointmentConfig.Add(config);
            await db!.SaveChangesAsync();

            var aggregatesForMonth = await sut!.SearchAppointmentsByYearMonthAsync(new YearMonth(2021, 5));
            Assert.AreEqual(new YearMonth(2021, 5), aggregatesForMonth.Month);
            Assert.IsFalse(aggregatesForMonth.PrevMonthAvailable);
            Assert.IsFalse(aggregatesForMonth.NextMonthAvailable);
            Assert.IsTrue(aggregatesForMonth.Appointments.Any());
            foreach (var daily in aggregatesForMonth.Appointments)
            {
                Assert.AreEqual(new YearMonth(2021, 5), daily.Date.ToYearMonth());
                if (daily.Date <= new LocalDate(2021, 5, 4))
                {
                    Assert.IsFalse(daily.PrevDateAvailable);
                    Assert.IsTrue(daily.NextDateAvailable);
                }
                else if (daily.Date >= new LocalDate(2021, 5, 6))
                {
                    Assert.IsTrue(daily.PrevDateAvailable);
                    Assert.IsFalse(daily.NextDateAvailable);
                }
                else
                {
                    Assert.IsFalse(daily.PrevDateAvailable);
                    Assert.IsFalse(daily.NextDateAvailable);
                }
            }
        }

        [TestMethod]
        public async Task SearchAppointmentsByYearMonthAsync_should_return_aggregate_given_prev_month_available()
        {
            var config = new AppointmentConfig()
            {
                AvailableIntervalStart = new LocalDate(2021, 4, 30),
                AvailableIntervalEnd = new LocalDate(2021, 5, 5),
            };
            db!.AppointmentConfig.Add(config);
            await db!.SaveChangesAsync();

            var aggregatesForMonth = await sut!.SearchAppointmentsByYearMonthAsync(new YearMonth(2021, 5));
            Assert.AreEqual(new YearMonth(2021, 5), aggregatesForMonth.Month);
            Assert.IsTrue(aggregatesForMonth.PrevMonthAvailable);
            Assert.IsFalse(aggregatesForMonth.NextMonthAvailable);
        }

        [TestMethod]
        public async Task SearchAppointmentsByYearMonthAsync_should_return_aggregate_given_next_month_available()
        {
            var config = new AppointmentConfig()
            {
                AvailableIntervalStart = new LocalDate(2021, 5, 1),
                AvailableIntervalEnd = new LocalDate(2021, 6, 1),
            };
            db!.AppointmentConfig.Add(config);
            await db!.SaveChangesAsync();

            var aggregatesForMonth = await sut!.SearchAppointmentsByYearMonthAsync(new YearMonth(2021, 5));
            Assert.AreEqual(new YearMonth(2021, 5), aggregatesForMonth.Month);
            Assert.IsFalse(aggregatesForMonth.PrevMonthAvailable);
            Assert.IsTrue(aggregatesForMonth.NextMonthAvailable);
        }

        [TestMethod]
        public async Task SearchAppointmentsByYearMonthAsync_should_return_aggregate_given_slot()
        {
            db!.AppointmentConfig.Add(new AppointmentConfig()
            {
                AvailableIntervalStart = new LocalDate(2021, 1, 1),
                AvailableIntervalEnd = new LocalDate(2021, 12, 31),
            });

            db!.Slots.Add(new AppointmentSlot()
            {
                From = new LocalDateTime(2021, 5, 1, 10, 0),
                Duration = Period.FromHours(1),
                CountOfSlot = 1,
            });
            await db!.SaveChangesAsync();

            var aggregatesForMonth = await sut!.SearchAppointmentsByYearMonthAsync(new YearMonth(2021, 5));
            Assert.IsTrue(aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AvailableSlots.Any());
            Assert.AreEqual(new LocalDateTime(2021, 5, 1, 10, 0), aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AvailableSlots.First().From);
            Assert.IsFalse(aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AvailableSlots.First().Appointments.Any());
        }

        [TestMethod]
        public async Task SearchAppointmentsByYearMonthAsync_should_return_aggregate_given_slot_and_appointment()
        {
            db!.AppointmentConfig.Add(new AppointmentConfig()
            {
                AvailableIntervalStart = new LocalDate(2021, 1, 1),
                AvailableIntervalEnd = new LocalDate(2021, 12, 31),
            });

            db!.Slots.Add(new AppointmentSlot()
            {
                From = new LocalDateTime(2021, 5, 1, 10, 0),
                Duration = Period.FromHours(1),
                CountOfSlot = 1,
            });
            db!.Appointments.Add(new Appointment()
            {
                From = new LocalDateTime(2021, 5, 1, 10, 0),
                Duration = Period.FromHours(1),
            });
            await db!.SaveChangesAsync();

            var aggregatesForMonth = await sut!.SearchAppointmentsByYearMonthAsync(new YearMonth(2021, 5));
            Assert.IsTrue(aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AvailableSlots.Any());
            Assert.AreEqual(new LocalDateTime(2021, 5, 1, 10, 0), aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AvailableSlots.First().From);
            Assert.IsTrue(aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AvailableSlots.First().Appointments.Any());
        }

        [TestMethod]
        public async Task CreateAppointmentSlotAsync_should_return_success()
        {
            var result = await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            Assert.IsTrue(result.Succeeded);

            var slot = await db!.Slots.FirstAsync();
            Assert.AreEqual(new LocalDateTime(2021, 5, 1, 10, 0), slot.From);
            Assert.AreEqual(Period.FromHours(1), slot.Duration);
            Assert.AreEqual(1, slot.CountOfSlot);
        }
    }
}
