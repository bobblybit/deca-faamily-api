using DatabaseBackup.HostedServices;
using DatabaseBackup.Repository;
using DatabaseBackup.Services.Implementation;
using DatabaseBackup.Services.Interface;
using DatabaseBackup.Settings;

using MassTransit;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DatabaseBackup
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
            services.Configure<ConnectionStringsConfig>(Configuration.GetSection(ConnectionStringsConfig.ConnectionStringsSettingsSection));
            services.Configure<DropBoxConfig>(Configuration.GetSection(DropBoxConfig.DropBoxSettings));
            services.AddScoped<IDropBoxService, DropBoxService>();
            services.AddScoped<IBackupRepository, BackupRepository>();
            services.AddSingleton<IFilesService, FilesService>();
            services.AddScoped<IBackupService, BackupService>();
            services.AddHostedService<DatabaseBackupHostedService>();

            services.AddMassTransit(x => {
                x.UsingRabbitMq((ctx, config) =>
                {
                    config.Host("amqp://guest:guest@localhost:5672");
                });
            });
            services.AddMassTransitHostedService();

            services.AddControllers();
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
