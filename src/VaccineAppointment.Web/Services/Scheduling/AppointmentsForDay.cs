using NodaTime;
using System.Collections.Generic;
using System.Linq;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public class AppointmentsForDay
    {
        public AppointmentsForDay(LocalDate date, AppointmentConfig? config, List<AppointmentAggregate> slots)
        {
            PrevDateAvailable = config == null || config.AvailableIntervalStart < date;
            NextDateAvailable = config == null || date < config.AvailableIntervalEnd;
            Date = date;
            AvailableSlots = slots.OrderBy(a => a.From).ToList();
        }

        public LocalDate Date { get; }
        public bool PrevDateAvailable { get; set; }
        public bool NextDateAvailable { get; set; }
        public List<AppointmentAggregate> AvailableSlots { get; }
    }
}
