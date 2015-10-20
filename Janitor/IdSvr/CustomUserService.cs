using Janitor.AspId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.AspNetIdentity;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;

namespace Janitor.IdSvr
{
    public static class CustomUserServiceExtensions
    {
        public static void ConfigureCustomUserService(this IdentityServerServiceFactory factory, string connString)
        {
            factory.UserService = new Registration<IUserService, CustomUserService>();
            factory.Register(new Registration<CustomUserManager>());
            factory.Register(new Registration<CustomUserStore>());
            factory.Register(new Registration<CustomContext>(resolver => new CustomContext(connString)));
        }
    }

    public class CustomUserService : AspNetIdentityUserService<CustomUser, int>
    {
        public CustomUserService(CustomUserManager userMgr)
            : base(userMgr)
        {
        }
    }
}
