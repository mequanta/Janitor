using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace Janitor
{
    public class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                 new Client
                {
                    ClientName = "Test Client",
                    ClientId = "test",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    // server to server communication
                    Flow = Flows.ClientCredentials,
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://janitor.chinacloudsites.cn"
                    },
                    // only allowed to access api1
                    AllowedScopes = new List<string>
                    {
                        "api1"
                    }
                },

                new Client
                {
                    ClientName = "Silicon on behalf of Carbon Client",
                    ClientId = "carbon",
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,
                    Flow = Flows.ResourceOwner,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("carbonsecret".Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "api1"
                    }
                }
            };
        }
    }
}