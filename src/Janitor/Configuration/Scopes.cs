using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace Janitor
{
    public class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            return new[]
                {
                    StandardScopes.OpenId,
                    StandardScopes.Profile,
                    StandardScopes.Email,
                    StandardScopes.OfflineAccess,

                    new Scope
                    {
                        Name = "read",
                        DisplayName = "Read data",
                        Type = ScopeType.Resource,
                        Emphasize = false,
                    },
                    new Scope
                    {
                        Name = "write",
                        DisplayName = "Write data",
                        Type = ScopeType.Resource,
                        Emphasize = true,
                    },
                    new Scope
                    {
                        Name = "api1",
                        DisplayName = "API 1",
                        Type = ScopeType.Resource,

                        Claims= new List<ScopeClaim>
                        {
                            new ScopeClaim("role")
                        }
                   
                    },
                    new Scope
                    {
                        Name = "testapi",
                        DisplayName = "Test Api",
                        Type = ScopeType.Resource,
                        Claims= new List<ScopeClaim>
                        {
                            new ScopeClaim("role")
                        }
                    }
                };
        }
    }
}