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
                    ClientName = "Client credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("373f4671-0c18-48d6-9da3-962b1c81299a".Sha256()) },

                    AllowedScopes = { "api1" }
                }
            };
        }
    }
}