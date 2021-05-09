﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VaccineAppointment.Web.Infrastructure;

namespace VaccineAppointment.Web.Migrations
{
    [DbContext(typeof(VaccineAppointmentContext))]
    partial class VaccineAppointmentContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VaccineAppointment.Web.Infrastructure.Models.Configuration", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Json")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("VaccineAppointment.Web.Models.Mailing.EmailTemplate", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BccTemplate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BodyTemplate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CcTemplate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FromTemplate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubjectTemplate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TemplateName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ToTemplate")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("EmailTemplates");
                });

            modelBuilder.Entity("VaccineAppointment.Web.Models.Scheduling.Appointment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("AppointmentSlotId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("From")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sex")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("From");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("VaccineAppointment.Web.Models.Scheduling.AppointmentSlot", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CountOfSlot")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<DateTime>("From")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Slots");
                });

            modelBuilder.Entity("VaccineAppointment.Web.Models.Users.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}