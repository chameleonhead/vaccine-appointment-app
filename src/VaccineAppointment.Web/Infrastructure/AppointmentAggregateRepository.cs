using Microsoft.EntityFrameworkCore;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Infrastructure
{
    public class AppointmentAggregateRepository : IAppointmentAggregateRepository
    {
        private readonly VaccineAppointmentContext db;

        public AppointmentAggregateRepository(VaccineAppointmentContext db)
        {
            this.db = db;
        }

        public async Task<AppointmentAggregate?> FindBySlotIdAsync(string id)
        {
            var slot = await db.Slots.FirstOrDefaultAsync(slot => slot.Id == id);
            if (slot == null)
            {
                return null;
            }
            var endTime = slot.From + slot.Duration;
            var appointments = await db.Appointments.Where(a => slot.From <= a.From && a.From <= endTime).ToListAsync();
            return new AppointmentAggregate(slot, appointments);
        }

        public async Task<AppointmentAggregate?> FindByAppointmentIdAsync(string id)
        {
            var appointment = await db.Appointments.FirstOrDefaultAsync(app => app.Id == id);
            if (appointment == null)
            {
                return null;
            }
            return await FindBySlotIdAsync(appointment.AppointmentSlotId);
        }

        public async Task<List<AppointmentAggregate>> SearchAsync(LocalDate from, LocalDate to)
        {
            var slots = await db.Slots.Where(slot => from.AtMidnight() <= slot.From && slot.From < to.PlusDays(1).AtMidnight()).ToListAsync();
            var appointments = await db.Appointments.Where(appointment => from.AtMidnight() <= appointment.From && appointment.From < to.PlusDays(1).AtMidnight()).ToListAsync();
            return slots.Select(s => new AppointmentAggregate(s, appointments.Where(a => s.From <= a.From && a.From <= s.From + s.Duration))).ToList();
        }

        public async Task AddAsync(AppointmentAggregate appointmentAggregate)
        {
            using var trans = await db.Database.BeginTransactionAsync();
            await db.Slots.AddAsync(appointmentAggregate.Slot);
            await db.Appointments.AddRangeAsync(appointmentAggregate.Appointments);
            await db.SaveChangesAsync();
            await trans.CommitAsync();
        }

        public async Task UpdateAsync(AppointmentAggregate appointmentAggregate)
        {
            using var trans = await db.Database.BeginTransactionAsync();
            db.Slots.Update(appointmentAggregate.Slot);
            await db.SaveChangesAsync();

            db.Appointments.RemoveRange(db.Appointments.Where(a => a.AppointmentSlotId == appointmentAggregate.Id));
            await db.Appointments.AddRangeAsync(appointmentAggregate.Appointments);
            await db.SaveChangesAsync();
            await trans.CommitAsync();
        }

        public async Task RemoveAsync(string id)
        {
            using var trans = await db.Database.BeginTransactionAsync();
            var slot = await db.Slots.FirstOrDefaultAsync(s => s.Id == id);
            db.Slots.Remove(slot);
            await db.SaveChangesAsync();

            if (await db.Appointments.AnyAsync(a => a.AppointmentSlotId == id))
            {
                await trans.RollbackAsync();
                throw new InvalidOperationException("appointment still exists");
            }
            await trans.CommitAsync();
        }

        public async Task AddRangeAsync(List<AppointmentAggregate> aggregates)
        {
            using var trans = await db.Database.BeginTransactionAsync();
            await db.Slots.AddRangeAsync(aggregates.Select(a => a.Slot));
            await db.Appointments.AddRangeAsync(aggregates.SelectMany(a => a.Appointments));
            await db.SaveChangesAsync();
            await trans.CommitAsync();
        }
    }
}
