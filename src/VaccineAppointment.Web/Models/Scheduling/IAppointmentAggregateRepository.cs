using NodaTime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public interface IAppointmentAggregateRepository
    {
        Task<AppointmentAggregate?> FindBySlotIdAsync(string id);
        Task<AppointmentAggregate?> FindByAppointmentIdAsync(string id);
        Task<List<AppointmentAggregate>> SearchAsync(LocalDate from, LocalDate to);
        Task AddAsync(AppointmentAggregate appointmentAggregate);
        Task UpdateAsync(AppointmentAggregate appointmentAggregate);
    }
}
