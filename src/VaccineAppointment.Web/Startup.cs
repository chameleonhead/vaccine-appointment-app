using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using VaccineAppointment.Web.Infrastructure;
using VaccineAppointment.Web.Models.Mailing;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Models.Users;
using VaccineAppointment.Web.Services.Mailing;
using VaccineAppointment.Web.Services.Scheduling;
using VaccineAppointment.Web.Services.Users;

namespace VaccineAppointment.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Logout";
                });

            services.AddDbContext<VaccineAppointmentContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection"));
            });
            services.AddSingleton<IPasswordHasher>(new PasswordHasher(Configuration.GetValue<string>("VaccineAppointment.Web:PasswordSalt")));
            services.AddTransient<IEmailConfigManager, ConfigurationManager>();
            services.AddTransient<IAppointmentConfigManager, ConfigurationManager>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddTransient<IAppointmentAggregateRepository, AppointmentAggregateRepository>();
            services.AddTransient<EmailService>();
            services.AddTransient<AppointmentService>();
            services.AddTransient<UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<VaccineAppointmentContext>())
                {
                    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
                    db.Database.EnsureCreated();
                    if (!db.Users.Any())
                    {
                        db.Users.Add(new User()
                        {
                            Username = "admin",
                            Password = hasher.Hash("P@ssw0rd"),
                            Role = "Administrator",
                        });
                        db.SaveChanges();
                    }
                    if (!db.EmailTemplates.Any())
                    {
                        db.EmailTemplates.Add(new EmailTemplate()
                        {
                            Id = Guid.NewGuid().ToString(),
                            TemplateName = "AppointmentAcceptedMessage",
                            FromTemplate = "admin@example.com",
                            SubjectTemplate = "【ワクチン予約Webサイト】ご予約ありがとうございます。",
                            BodyTemplate = "{{Name}}様\r\n\r\n当システムをご利用いただき、誠にありがとうございます。\r\n予約を以下の通り承りました。\r\n\r\n予約ID: {{AppointmentId}}\r\n予約日: {{Date}}\r\nお時間: {{FromTime}} - {{ToTime}}\r\n\r\n当日は所定の時間までにお越しください。\r\n\r\n本メールには返信してもお返事が出来ませんのでご了承願います。\r\n\r\n----------------------------------\r\nワクチン予約Webサイト\r\n",
                        });
                        db.SaveChanges();
                    }
                }
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
