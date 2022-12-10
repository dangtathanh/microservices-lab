using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace GRPCLab.IdentityService.Configuration
{
    public class Config
    {
        public static readonly string ApiClientName = "grpclab-api";

        // ApiScopes define the apis in your system
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("contact", "Contact resources"),
                new ApiScope("message", "Message resources")
            };
        }

        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("profile", "Profile Service")
                {
                    Scopes =
                    {
                        "profile"
                    }
                },
                new ApiResource("contact", "Contact Service")
                {
                    Scopes =
                    {
                        "contact"
                    }
                },
                new ApiResource("message", "Message Service")
                {
                    Scopes =
                    {
                        "message"
                    }
                }
            };
        }

        // Client wants to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            var settings = new AppSettings();
            ConfigurationBinder.Bind(configuration, settings);
            var clients = new List<Client>
            {
                new Client
                {
                    ClientId = ApiClientName,
                    ClientName = "GrpcLab Client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(settings.ApiClientSecrect.Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "contact",
                        "message"
                    },
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    UpdateAccessTokenClaimsOnRefresh = true
                },
            };
            return clients;
        }
    }
}
