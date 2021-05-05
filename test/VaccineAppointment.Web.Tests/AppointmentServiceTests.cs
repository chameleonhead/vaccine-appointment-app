using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using System.Collections.Generic;
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
            Assert.IsTrue(aggregatesForDay.PrevDateAvailable);
            Assert.IsTrue(aggregatesForDay.NextDateAvailable);
            Assert.IsFalse(aggregatesForDay.AllSlots.Any());
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
            Assert.IsFalse(aggregatesForDay.AllSlots.Any());
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
            Assert.IsFalse(aggregatesForDay.AllSlots.Any());
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
            Assert.IsFalse(aggregatesForDay.AllSlots.Any());
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
            Assert.IsTrue(aggregatesForDay.AllSlots.Any());
            Assert.IsFalse(aggregatesForDay.AllSlots.First().Appointments.Any());
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
            Assert.IsTrue(aggregatesForDay.AllSlots.First().Appointments.Any());
        }

        [TestMethod]
        public async Task SearchAppointmentsByYearMonthAsync_should_return_aggregate_given_no_slot_no_config()
        {
            var aggregatesForMonth = await sut!.SearchAppointmentsByYearMonthAsync(new YearMonth(2021, 5));
            Assert.AreEqual(new YearMonth(2021, 5), aggregatesForMonth.Month);
            Assert.IsTrue(aggregatesForMonth.PrevMonthAvailable);
            Assert.IsTrue(aggregatesForMonth.NextMonthAvailable);
            Assert.IsTrue(aggregatesForMonth.Appointments.Any());
            foreach (var daily in aggregatesForMonth.Appointments)
            {
                Assert.AreEqual(new YearMonth(2021, 5), daily.Date.ToYearMonth());
                Assert.IsTrue(daily.PrevDateAvailable);
                Assert.IsTrue(daily.NextDateAvailable);
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
            Assert.IsTrue(aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AllSlots.Any());
            Assert.AreEqual(new LocalDateTime(2021, 5, 1, 10, 0), aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AllSlots.First().From);
            Assert.IsFalse(aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AllSlots.First().Appointments.Any());
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
            Assert.IsTrue(aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AllSlots.Any());
            Assert.AreEqual(new LocalDateTime(2021, 5, 1, 10, 0), aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AllSlots.First().From);
            Assert.IsTrue(aggregatesForMonth.Appointments.First(e => e.Date == new LocalDate(2021, 5, 1)).AllSlots.First().Appointments.Any());
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

        [TestMethod]
        public async Task CreateAppointmentSlotAsync_should_return_fail_given_slot_already_exists()
        {
            await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            var result = await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("ó\ñÒògÇ™èdï°ÇµÇƒÇ¢Ç‹Ç∑ÅB", result.ErrorMessage);
        }

        [TestMethod]
        public async Task CreateAppointmentSlotAsync_should_return_fail_given_slot_not_overlap()
        {
            await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            var result = await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 9, 0), Period.FromHours(1), 1);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task CreateAppointmentSlotAsync_should_return_fail_given_slot_not_overlap2()
        {
            await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            var result = await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 11, 0), Period.FromHours(1), 1);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task UpdateAppointmentSlotAsync_should_return_success()
        {
            var createResult = await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            var id = (createResult as CreateAppointmentSlotResult)!.Id;

            var result = await sut!.UpdateAppointmentSlotAsync(id, new LocalDateTime(2021, 5, 1, 11, 0), Period.FromHours(1), 1);
            Assert.IsTrue(result.Succeeded);

            var slot = await db!.Slots.FirstAsync(s => s.Id == id);
            Assert.AreEqual(new LocalDateTime(2021, 5, 1, 11, 0), slot.From);
            Assert.AreEqual(Period.FromHours(1), slot.Duration);
            Assert.AreEqual(1, slot.CountOfSlot);
        }

        [TestMethod]
        public async Task UpdateAppointmentSlotAsync_should_return_fail_given_slot_not_exists()
        {
            var result = await sut!.UpdateAppointmentSlotAsync("unknownid", new LocalDateTime(2021, 5, 1, 9, 0), Period.FromHours(1), 1);
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("ó\ñÒògÇ™ë∂ç›ÇµÇ‹ÇπÇÒÅB", result.ErrorMessage);
        }

        [TestMethod]
        public async Task UpdateAppointmentSlotAsync_should_return_fail_given_slot_already_exists()
        {
            await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 9, 0), Period.FromHours(1), 1);
            var createResult = await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            var id = (createResult as CreateAppointmentSlotResult)!.Id;

            var result = await sut!.UpdateAppointmentSlotAsync(id, new LocalDateTime(2021, 5, 1, 9, 0), Period.FromHours(1), 1);
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("ó\ñÒògÇ™èdï°ÇµÇƒÇ¢Ç‹Ç∑ÅB", result.ErrorMessage);
        }

        [TestMethod]
        public async Task UpdateAppointmentSlotAsync_should_return_fail_given_appointment_exists()
        {
            var createResult = await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            var id = (createResult as CreateAppointmentSlotResult)!.Id;
            await sut!.CreateAppointmentAsync(id, "User 1", "user@example.com", "F", 10);

            var result = await sut!.UpdateAppointmentSlotAsync(id, new LocalDateTime(2021, 5, 1, 11, 0), Period.FromHours(1), 1);
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("ó\ñÒògÇ…ó\ñÒÇ™Ç†ÇÈÇΩÇﬂçXêVÇ≈Ç´Ç‹ÇπÇÒÅB", result.ErrorMessage);
        }

        [TestMethod]
        public async Task UpdateAppointmentSlotAsync_should_return_fail_given_slot_not_overlap()
        {
            await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 9, 0), Period.FromHours(1), 1);
            var createResult = await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            var id = (createResult as CreateAppointmentSlotResult)!.Id;

            var result = await sut!.UpdateAppointmentSlotAsync(id, new LocalDateTime(2021, 5, 1, 11, 0), Period.FromHours(1), 1);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task DeleteAppointmentSlotAsync_should_return_success()
        {
            var createResult = await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            var id = (createResult as CreateAppointmentSlotResult)!.Id;

            var result = await sut!.DeleteAppointmentSlotAsync(id);
            Assert.IsTrue(result.Succeeded);

            Assert.IsFalse(await db!.Slots.AnyAsync(s => s.Id == id));
        }

        [TestMethod]
        public async Task DeleteAppointmentSlotAsync_should_return_fail_given_slot_not_exists()
        {
            var result = await sut!.DeleteAppointmentSlotAsync("unknownid");
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("ó\ñÒògÇ™ë∂ç›ÇµÇ‹ÇπÇÒÅB", result.ErrorMessage);
        }

        [TestMethod]
        public async Task DeleteAppointmentSlotAsync_should_return_fail_given_appointment_exists()
        {
            var createResult = await sut!.CreateAppointmentSlotAsync(new LocalDateTime(2021, 5, 1, 10, 0), Period.FromHours(1), 1);
            var id = (createResult as CreateAppointmentSlotResult)!.Id;
            await sut!.CreateAppointmentAsync(id, "User 1", "user@example.com", "F", 10);

            var result = await sut!.DeleteAppointmentSlotAsync(id);
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("ó\ñÒògÇ…ó\ñÒÇ™Ç†ÇÈÇΩÇﬂçÌèúÇ≈Ç´Ç‹ÇπÇÒÅB", result.ErrorMessage);
        }

        [TestMethod]
        public async Task CreateMultipleAppointmentSlotsAsync_should_return_ok()
        {
            var result = await sut!.CreateMultipleAppointmentSlotsAsync(new List<LocalDate>() { new LocalDate(2021, 5, 1) }, new LocalTime(10, 0), Period.FromHours(1), 1, 2);
            Assert.IsTrue(result.Succeeded);

            var slots = await db!.Slots.OrderBy(d => d.From).ToListAsync();
            var slot1 = slots.First();
            Assert.AreEqual(new LocalDateTime(2021, 5, 1, 10, 0), slot1.From);
            Assert.AreEqual(Period.FromHours(1), slot1.Duration);
            Assert.AreEqual(1, slot1.CountOfSlot);

            var slot2 = slots.Last();
            Assert.AreEqual(new LocalDateTime(2021, 5, 1, 11, 0), slot2.From);
            Assert.AreEqual(Period.FromHours(1), slot2.Duration);
            Assert.AreEqual(1, slot2.CountOfSlot);
        }
    }
}
