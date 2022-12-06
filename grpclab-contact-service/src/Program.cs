using GRPCLab.ContactService;
using GRPCLab.ContactService.Infrastructures.Extensions;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// App Configurations
builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
        .AddEnvironmentVariables();
});

// Configure Serilog
builder.WebHost.ConfigureSerilog(builder.Configuration);

// Bind appsettings.json to AppSettings
builder.Services.Configure<AppSettings>(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();

// Add API versioning
builder.Services.AddApiVersion();

// Add Swagger
builder.Services.AddSwagger();

// Add Services
builder.Services.AddProfileServices(builder.Configuration);

// Build the app
var app = builder.Build();

// Configure Swagger
app.ConfigureSwagger(builder.Environment);

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();