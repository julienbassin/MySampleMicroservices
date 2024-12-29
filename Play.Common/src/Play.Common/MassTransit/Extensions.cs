using System;
using System.Reflection;
using GreenPipes;
using GreenPipes.Configurators;
using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(
            this IServiceCollection services,
            Action<IRetryConfigurator> configurationRetries = null
            )
        {
            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());
                configure.UsingPlayEconomyRabbitMQ(configurationRetries);
            });

            services.AddMassTransitHostedService();

            return services;
        }

        public static void UsingPlayEconomyRabbitMQ( 
           this IServiceCollectionBusConfigurator configure,
           Action<IRetryConfigurator> configurationRetries = null)
        {
            configure.UsingRabbitMq((context, configurator) =>
            {
                var configuration = context.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                configurator.Host(rabbitMQSettings.Host);
                configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));

                if (configurationRetries == null)
                {
                    configurationRetries = (retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5)); ;
                }

                configurator.UseMessageRetry(configurationRetries);

            });
        }
    }
}