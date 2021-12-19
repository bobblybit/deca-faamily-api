using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MqDtos;
using System;
using System.Net;

namespace ArticleService.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if(contextFeature != null)
                    {
                        //Publish to Rabbitmq

                        var response = new ErrorDetailMqDto()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                            Source = contextFeature.Error.Source,
                            StackTrace = contextFeature.Error.StackTrace.Substring(0, 300)
                        };
                        
                        var _bus = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IBus>();

                        var endpoint = await _bus.GetSendEndpoint(new Uri("exchange:errors-queue"));
                        await endpoint.Send<ErrorDetailMqDto>(response);

                        response.Message = "Internal server error";
                        await context.Response.WriteAsync(response.ToString());
                    }
                });
            });
        }
    }
}