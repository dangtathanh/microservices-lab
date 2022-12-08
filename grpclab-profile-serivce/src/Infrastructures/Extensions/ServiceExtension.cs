using Autofac;
using GRPCLab.BuildingBlocks.EventBus;
using GRPCLab.BuildingBlocks.EventBus.Abstractions;
using GRPCLab.BuildingBlocks.EventBusRabbitMQ;
using GRPCLab.ProfileService.IntegrationEvents.EventHandling;
using RabbitMQ.Client;

namespace GRPCLab.ProfileService.Infrastructures.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterEventBus(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                var factory = new ConnectionFactory()
                {
                    HostName = configuration.GetValue<string>("EventBusConnection"),
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration.GetValue<string>("EventBusUserName")))
                {
                    factory.UserName = configuration.GetValue<string>("EventBusUserName");
                }

                if (!string.IsNullOrEmpty(configuration.GetValue<string>("EventBusPassword")))
                {
                    factory.Password = configuration.GetValue<string>("EventBusPassword");
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration.GetValue<string>("EventBusRetryCount")))
                {
                    retryCount = int.Parse(configuration.GetValue<string>("EventBusRetryCount"));
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });


            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var subscriptionClientName = configuration.GetValue<string>("SubscriptionClientName");
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration.GetValue<string>("EventBusRetryCount")))
                {
                    retryCount = int.Parse(configuration.GetValue<string>("EventBusRetryCount"));
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubscriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            services.AddTransient<ContactAddedIntegrationEventHandler>();

            return services;
        }
    }
}
