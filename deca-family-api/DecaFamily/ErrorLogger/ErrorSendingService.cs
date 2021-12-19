using MassTransit;
using Microsoft.Extensions.Hosting;
using MqDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ErrorLogger
{
    public class ErrorSendingService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IBus _bus;
        private readonly ExceptionListMqDto _exceptions;

        public ErrorSendingService(IBus bus, ExceptionListMqDto exceptions)
        {
            _bus = bus;
            _exceptions = exceptions;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            TimeSpan interval = TimeSpan.FromHours(24);

            //calculate time to run the first time & delay to set the timer
            //DateTime.Today gives time of midnight 00.00
            var nextRunTime = DateTime.Today.AddDays(1).AddHours(7);

            var curTime = DateTime.Now;
            var firstInterval = nextRunTime.Subtract(curTime);


            Action action = () =>
            {
                var t1 = Task.Delay(firstInterval);
                t1.Wait();

                //send errro logs mail at expected time
                SendErrorLogs(null);

                //now schedule it to be called every 24 hours for future
                // timer repeates call to SendErrorLogs every 24 hours.
                _timer = new Timer(
                    SendErrorLogs,
                    null,
                    TimeSpan.Zero,
                    interval
                );
            };

            Task.Run(action);
            return Task.CompletedTask;
        }

        /// Call the Stop async method if required from within the app.
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void SendErrorLogs(object state)
        {
            _bus.Publish<ExceptionListMqDto>(_exceptions).GetAwaiter();
            _exceptions.Errors.Clear();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
