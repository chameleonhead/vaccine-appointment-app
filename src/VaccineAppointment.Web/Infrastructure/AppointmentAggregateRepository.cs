using Microsoft.EntityFrameworkCore;
using NodaTime;
using System;
using System.Collections.Generic;
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
            return new AppointmentAggregate(slot);
        }

        public Task<List<AppointmentAggregate>> SearchAsync(LocalDate from, LocalDate to)
        {
            throw new NotImplementedException();
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
