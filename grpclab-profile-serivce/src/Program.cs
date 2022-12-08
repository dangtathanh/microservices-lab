using Autofac;
using Autofac.Extensions.DependencyInjection;
using GRPCLab.BuildingBlocks.EventBus.Abstractions;
using GRPCLab.ProfileService.Infrastructures.Extensions;
using GRPCLab.ProfileService.Infrastructures.Modules;
using GRPCLab.ProfileService.IntegrationEvents.EventHandling;
using GRPCLab.ProfileService.IntegrationEvents.Events;
using GRPCLab.ProfileService.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new ContainerModule()));

builder.WebHost.ConfigureKestrel(otp =>
{
    otp.Listen(System.Net.IPAddress.Any, 80,
                    o => o.Protocols = HttpProtocols.Http1AndHttp2);
    otp.Listen(System.Net.IPAddress.Any, 443, o =>
    {
        o.Protocols = HttpProtocols.Http2;
        o.UseHttps(builder.Configuration.GetValue<string>("CertPath"), builder.Configuration.GetValue<string>("CertPass"));
    });
});

// Add services to the container.
builder.Services.AddGrpc();

// Add Services
builder.Services.RegisterEventBus(builder.Configuration);

var app = builder.Build();

// Subcribe event
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<ContactAddedIntegrationEvent, ContactAddedIntegrationEventHandler>();

app.MapGrpcService<ProfileService>();
app.MapGet("/", () => "Hello World!");

app.Run();
