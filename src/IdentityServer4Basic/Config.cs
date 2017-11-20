using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer4Basic
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new[]
            {
                new ApiResource("api1", "My API #1")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1" }
                }
            };
        }
    }
}