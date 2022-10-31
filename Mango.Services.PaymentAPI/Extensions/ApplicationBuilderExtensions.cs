﻿using Mango.Services.PaymentAPI.Messaging;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Mango.Services.PaymentAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopped.Register(OnStop);
            return app;
        }
        private static void OnStart()
        {
            ServiceBusConsumer.Start();
        }
        private static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }
    }
}
