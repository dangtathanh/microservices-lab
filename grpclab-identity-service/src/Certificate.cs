using System.Security.Cryptography.X509Certificates;

namespace GRPCLab.IdentityService
{
    internal static class Certificate
    {
        public static X509Certificate2 Get(string password)
        {
            return new X509Certificate2("certs/certificate.pfx", password);
        }
    }
}
