
using System;
using System.Threading;
using System.Threading.Tasks;

using DatabaseBackup.Controllers;
using DatabaseBackup.Services;
using DatabaseBackup.Services.Interface;
using DatabaseBackup.Settings;

using Dropbox.Api;

using MassTransit;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MqDtos;

namespace DatabaseBackup.HostedServices
{
    public class DatabaseBackupHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IBus _bus;
        private readonly ConnectionStringsConfig _connectionStrings;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseBackupHostedService(IOptions<ConnectionStringsConfig> connectionStrings, IServiceProvider serviceProvider, IBus bus)
        {
            _connectionStrings = connectionStrings.Value;
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(BackupDatabase, null, TimeSpan.FromDays(30), TimeSpan.FromDays(30));
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void BackupDatabase(object obj)
        {
            string dropboxLink;
            try { 
                using(var scope = _serviceProvider.CreateScope())
                {
                    var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();
                    backupService.CreateBackup();
                    
                    var dropBoxService = scope.ServiceProvider.GetRequiredService<IDropBoxService>();
                    dropboxLink = dropBoxService.Upload().GetAwaiter().GetResult();
                }
                var emailDto = new BackupEmailDto { Date = DateTime.Now, Link = dropboxLink };
                PublishSuccessMessage(emailDto);
            }
            catch(Exception ex)
            {
                var exception = new ErrorDetailMqDto
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    StatusCode = 500
                };
                PublishExceptionMessage(exception);
            }
        }

        public void PublishSuccessMessage(BackupEmailDto emailDto)
        {
            _bus.Publish<BackupEmailDto>(emailDto);
        }

        public void PublishExceptionMessage(ErrorDetailMqDto ex)
        {
            var endpoint = _bus.GetSendEndpoint(new Uri("exchange:errors-queue")).GetAwaiter().GetResult();
            endpoint.Send<ErrorDetailMqDto>(ex).Wait();
        }
    }
}
