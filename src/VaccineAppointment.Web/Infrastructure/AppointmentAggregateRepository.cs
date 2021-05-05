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

        public async Task<List<AppointmentAggregate>> SearchAsync(LocalDate from, LocalDate to)
        {
            var slots = await db.Slots.Where(slot => from.AtMidnight() <= slot.From && slot.From < to.PlusDays(1).AtMidnight()).ToListAsync();
            var appointments = await db.Appointments.Where(appointment => from.AtMidnight() <= appointment.From && appointment.From < to.PlusDays(1).AtMidnight()).ToListAsync();
            return slots.Select(s => new AppointmentAggregate(s, appointments.Where(a => s.From <= a.From && a.From <= s.From + s.Duration))).ToList();
        }

        public Task AddAsync(AppointmentAggregate user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(AppointmentAggregate user)
        {
            throw new NotImplementedException();
        }
    }
}
