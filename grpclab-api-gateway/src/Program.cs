using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Serilog;
using Serilog.Formatting.Compact;

// Declare App Builder
var builder = WebApplication.CreateBuilder(args);

// App Configurations
builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config
        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
        .AddOcelotWithSwaggerSupport(hostingContext.HostingEnvironment, "OcelotRoutes")
        .AddOcelot("OcelotRoutes", hostingContext.HostingEnvironment)
        .AddEnvironmentVariables();
}).ConfigureLogging((hostingContext, logging) =>
{
    var loggerConfiguration = new LoggerConfiguration()
                      .ReadFrom.Configuration(builder.Configuration)
                      .Enrich.FromLogContext();
    if (!bool.TrueString.Equals(hostingContext.Configuration["LogMultiline"], StringComparison.OrdinalIgnoreCase))
    {
        loggerConfiguration.WriteTo.Console(new RenderedCompactJsonFormatter());
    }
    else
    {
        loggerConfiguration.WriteTo.Console(outputTemplate: "[{Timestamp:yyyyMMdd-HH:mm:ss} {Level:u3} ApiGateway] {Message:lj}{Data:lj}{NewLine}{Exception}");
    }

    logging.ClearProviders();
    logging.AddSerilog(loggerConfiguration.CreateLogger());
});


builder.Services.AddOcelot().AddPolly();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// App Run
var app = builder.Build();

app.UseRouting();
app.UseSwagger();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});
app.UseOcelot().Wait();

app.Run();
