using NodaTime;
using System.Collections.Generic;
using System.Linq;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentsForDay
    {
        public AppointmentsForDay(LocalDate date, AppointmentConfig config, List<AppointmentAggregate> slots)
        {
            PrevDateAvailable = config.AvailableIntervalStart == null || config.AvailableIntervalStart < date;
            NextDateAvailable = config.AvailableIntervalEnd == null || date < config.AvailableIntervalEnd;
            Date = date;
            AllSlots = slots.OrderBy(a => a.From).ToList();
            if (config == null || (config.AvailableIntervalStart <= date && date <= config.AvailableIntervalEnd))
            {
                AvailableSlots = AllSlots.Where(a => a.CanCreateAppointment).ToList();
            }
            else
            {
                AvailableSlots = new List<AppointmentAggregate>();
            }
        }

        public LocalDate Date { get; }
        public bool PrevDateAvailable { get; set; }
        public bool NextDateAvailable { get; set; }
        public List<AppointmentAggregate> AllSlots { get; }
        public List<AppointmentAggregate> AvailableSlots { get; }
    }
}
