using Autofac;
using GRPCLab.ContactService.Servives;
using GrpcProfileClient;
using GRPCLab.BuildingBlocks.EventBus;
using GRPCLab.BuildingBlocks.EventBus.Abstractions;
using GRPCLab.BuildingBlocks.EventBusRabbitMQ;
using System.Security.Cryptography.X509Certificates;
using RabbitMQ.Client;
using GRPCLab.ContactService.IntegrationEvents.EventHandling;

namespace GRPCLab.ContactService.Infrastructures.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddProfileServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var profileUrl = configuration.GetValue<string>("ProfileUrl");
            var certPath = configuration.GetValue<string>("CertPath");
            var certPass = configuration.GetValue<string>("CertPass");

            ArgumentNullException.ThrowIfNull(profileUrl);

            services.AddGrpcClient<Profile.ProfileClient>(o =>
            {
                o.Address = new Uri(profileUrl);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var hcl = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                hcl.ClientCertificates.Add(new X509Certificate2(certPath, certPass));
                return hcl;
            });

            services.AddTransient<IProfileService<Servives.V1.ProfileService>, Servives.V1.ProfileService>();

            return services;
        }

        public static IServiceCollection RegisterEventBus(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                logger.LogInformation($"DEBUG: {configuration.GetValue<string>("EventBusConnection")}, {configuration.GetValue<string>("EventBusUserName")}, {configuration.GetValue<string>("EventBusPassword")}");
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
