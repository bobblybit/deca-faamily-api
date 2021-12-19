using MassTransit;
using MqDtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErrorLogger.ConsumerClasses
{
    public class ErrorDetailConsumer : IConsumer<ErrorDetailMqDto>
    {
        private readonly ExceptionListMqDto _exceptions;

        public ErrorDetailConsumer(ExceptionListMqDto exceptions)
        {
            _exceptions = exceptions;
        }

        public async Task Consume(ConsumeContext<ErrorDetailMqDto> context)
        {
            var error = new Error { StatusCode = context.Message.StatusCode, Message = context.Message.Message,
                                    Source = context.Message.Source,StackTrace = context.Message.StackTrace, 
                                    DateCreated = context.Message.DateCreated};


            _exceptions.Errors.Add(error);

        }
    }
}
