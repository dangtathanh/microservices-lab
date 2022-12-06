using GRPCLab.ProfileService.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.MapGrpcService<ProfileService>();
app.MapGet("/", () => "Hello World!");

app.Run();
