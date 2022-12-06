using GRPCLab.ContactService.Infrastructures.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using Serilog.Formatting.Compact;

namespace GRPCLab.ContactService.Infrastructures.Extensions
{
    public static class WebHostExtension
    {
        public static IServiceCollection AddApiVersion(this IServiceCollection services)
        {
            services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
            }).AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            // Add Swagger
            services.AddSwaggerGen(opt =>
            {
                // Add a custom operation filter which sets path to lowercase
                opt.DocumentFilter<LowercaseDocumentFilter>();

                // Add a custom operation filter which sets default values
                opt.OperationFilter<SwaggerDefaultValues>();
            });
            services.ConfigureOptions<ConfigureSwaggerOptions>();

            return services;
        }

        public static IWebHostBuilder ConfigureSerilog(this IWebHostBuilder builder, ConfigurationManager configuration)
        {
            builder.ConfigureLogging((hostingContext, logging) =>
            {
                var loggerConfiguration = new LoggerConfiguration()
                                               .ReadFrom.Configuration(configuration)
                                               .Enrich.FromLogContext();
                if (!bool.TrueString.Equals(hostingContext.Configuration["LogMultiline"], StringComparison.OrdinalIgnoreCase))
                {
                    loggerConfiguration.WriteTo.Console(new RenderedCompactJsonFormatter());
                }
                else
                {
                    loggerConfiguration.WriteTo.Console(outputTemplate: "[{Timestamp:yyyyMMdd-HH:mm:ss} {Level:u3} ContactService] {Message:lj}{Data:lj}{NewLine}{Exception}");
                }

                logging.ClearProviders();
                logging.AddSerilog(loggerConfiguration.CreateLogger());
            });
            return builder;
        }

        public static WebApplication ConfigureSwagger(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                if (app.Environment.IsDevelopment())
                {
                    var provider = app.Services.GetService<IApiVersionDescriptionProvider>();
                    app.UseSwagger();
                    app.UseSwaggerUI(opt =>
                    {
                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            opt.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                        }
                    });
                }
            }

            return app;
        }
    }
}
