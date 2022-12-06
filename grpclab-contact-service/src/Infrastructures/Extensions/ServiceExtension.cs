using GRPCLab.ContactService.Servives;
using GrpcProfileClient;
using System.Security.Cryptography.X509Certificates;

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
    }
}
