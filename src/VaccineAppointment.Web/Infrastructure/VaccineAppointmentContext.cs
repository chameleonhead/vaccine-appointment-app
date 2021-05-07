using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using System;
using VaccineAppointment.Web.Infrastructure.Models;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Models.Users;

namespace VaccineAppointment.Web.Infrastructure
{
    public class VaccineAppointmentContext : DbContext
    {
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public VaccineAppointmentContext(DbContextOptions<VaccineAppointmentContext> options) : base(options)
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<AppointmentSlot> Slots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Configuration> Configurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var localDateConverter =
                new ValueConverter<LocalDate, DateTime>(v =>
                    v.ToDateTimeUnspecified(),
                    v => LocalDate.FromDateTime(v));

            var localDateTimeConverter =
                new ValueConverter<LocalDateTime, DateTime>(v =>
                    v.ToDateTimeUnspecified(),
                    v => LocalDateTime.FromDateTime(v));

            var periodConverter =
                new ValueConverter<Period, TimeSpan>(v =>
                    v.ToDuration().ToTimeSpan(),
                    v => Period.FromTicks(v.Ticks));

            modelBuilder.Entity<AppointmentSlot>()
                .Property(e => e.From)
                .HasConversion(localDateTimeConverter);
            modelBuilder.Entity<AppointmentSlot>()
                .Property(e => e.Duration)
                .HasConversion(periodConverter);

            modelBuilder.Entity<Appointment>()
                .HasIndex(e => e.From);
            modelBuilder.Entity<Appointment>()
                .Property(e => e.From)
                .HasConversion(localDateTimeConverter);
            modelBuilder.Entity<Appointment>()
                .Property(e => e.Duration)
                .HasConversion(periodConverter);
        }
    }
}
