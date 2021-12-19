using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Settings;
using Microsoft.Extensions.Hosting;
using NotificationService.Interface;
using NotificationService.Implementation;
using MassTransit;
using NotificationService.ConsumerClasses;

namespace NotificationService
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
            services.AddMassTransit(x =>
            {
                x.AddConsumer<NewUserConsumer>();
                x.AddConsumer<ExceptionListConsumer>();
                x.AddConsumer<ResetPasswordConsumer>();
                x.AddConsumer<DatabaseBackupConsumer>();
                x.AddConsumer<RoleNotificationConsumer>();
                x.UsingRabbitMq((ctx, config) =>
                {
                    config.Host("amqp://guest:guest@localhost:5672");
                    config.ReceiveEndpoint("notification-queue", x =>
                    {
                        x.ConfigureConsumer<NewUserConsumer>(ctx);
                        x.ConfigureConsumer<ExceptionListConsumer>(ctx);
                        x.ConfigureConsumer<ResetPasswordConsumer>(ctx);
                        x.ConfigureConsumer<DatabaseBackupConsumer>(ctx);
                        x.ConfigureConsumer<RoleNotificationConsumer>(ctx);
                    });
                });
            });
            services.AddMassTransitHostedService();

            services.AddControllers();
            services.Configure<EmailSenderConfiguration>(Configuration.GetSection(EmailSenderConfiguration.ConfigSection));
            services.AddScoped<IEmailNotification, EmailNotification>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
