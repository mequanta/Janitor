using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Security.Claims;
using IdSvr3 = IdentityServer3.Core;
using IdentityManager;
using IdentityManager.AspNetIdentity;
using IdentityManager.Configuration;

namespace Janitor
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Role : IdentityRole { }

    public class Context : IdentityDbContext<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public Context(string connString)
            : base(connString)
        {
        }
    }

    public class UserStore : UserStore<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public UserStore(Context ctx)
            : base(ctx)
        {
        }
    }

    public class UserManager : UserManager<User, string>
    {
        public UserManager(UserStore store)
            : base(store)
        {
            this.ClaimsIdentityFactory = new ClaimsFactory();
        }
    }

    public class ClaimsFactory : ClaimsIdentityFactory<User, string>
    {
        public ClaimsFactory()
        {
            this.UserIdClaimType = IdSvr3.Constants.ClaimTypes.Subject;
            this.UserNameClaimType = IdSvr3.Constants.ClaimTypes.PreferredUserName;
            this.RoleClaimType = IdSvr3.Constants.ClaimTypes.Role;
        }

        public override async System.Threading.Tasks.Task<System.Security.Claims.ClaimsIdentity> CreateAsync(UserManager<User, string> manager, User user, string authenticationType)
        {
            var ci = await base.CreateAsync(manager, user, authenticationType);
            if (!String.IsNullOrWhiteSpace(user.FirstName))
            {
                ci.AddClaim(new Claim("given_name", user.FirstName));
            }
            if (!String.IsNullOrWhiteSpace(user.LastName))
            {
                ci.AddClaim(new Claim("family_name", user.LastName));
            }
            return ci;
        }
    }

    public class RoleStore : RoleStore<Role>
    {
        public RoleStore(Context ctx)
            : base(ctx)
        {
        }
    }

    public class RoleManager : RoleManager<Role>
    {
        public RoleManager(RoleStore store)
            : base(store)
        {
        }
    }


    public class CustomUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim> { }
    public class CustomUserLogin : IdentityUserLogin<int> { }
    public class CustomUserRole : IdentityUserRole<int> { }
    public class CustomUserClaim : IdentityUserClaim<int> { }

    public class CustomRole : IdentityRole<int, CustomUserRole> { }

    public class CustomContext : IdentityDbContext<CustomUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomContext(string connString)
            : base(connString)
        {
        }
    }

    public class CustomUserStore : UserStore<CustomUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(CustomContext ctx)
            : base(ctx)
        {
        }
    }

    public class CustomUserManager : UserManager<CustomUser, int>
    {
        public CustomUserManager(CustomUserStore store)
            : base(store)
        {
        }
    }

    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {
        public CustomRoleStore(CustomContext ctx)
            : base(ctx)
        {
        }
    }

    public class CustomRoleManager : RoleManager<CustomRole, int>
    {
        public CustomRoleManager(CustomRoleStore store)
            : base(store)
        {
        }
    }

    public static class SimpleIdentityManagerServiceExtensions
    {
        public static void ConfigureSimpleIdentityManagerService(this IdentityManagerServiceFactory factory, string connectionString)
        {
            factory.Register(new Registration<Context>(resolver => new Context(connectionString)));
            factory.Register(new Registration<UserStore>());
            factory.Register(new Registration<RoleStore>());
            factory.Register(new Registration<UserManager>());
            factory.Register(new Registration<RoleManager>());
            factory.IdentityManagerService = new Registration<IIdentityManagerService, SimpleIdentityManagerService>();
        }
    }

    public class SimpleIdentityManagerService : AspNetIdentityManagerService<User, string, Role, string>
    {
        public SimpleIdentityManagerService(UserManager userMgr, RoleManager roleMgr)
            : base(userMgr, roleMgr)
        {
        }
    }

    public static class CustomIdentityManagerServiceWithIntKeysExtensions
    {
        public static void ConfigureCustomIdentityManagerServiceWithIntKeys(this IdentityManagerServiceFactory factory, string connectionString)
        {
            factory.Register(new Registration<CustomContext>(resolver => new CustomContext(connectionString)));
            factory.Register(new Registration<CustomUserStore>());
            factory.Register(new Registration<CustomRoleStore>());
            factory.Register(new Registration<CustomUserManager>());
            factory.Register(new Registration<CustomRoleManager>());
            factory.IdentityManagerService = new Registration<IIdentityManagerService, CustomIdentityManagerServiceWithIntKeys>();
        }
    }

    public class CustomIdentityManagerServiceWithIntKeys : AspNetIdentityManagerService<CustomUser, int, CustomRole, int>
    {
        public CustomIdentityManagerServiceWithIntKeys(CustomUserManager userMgr, CustomRoleManager roleMgr)
            : base(userMgr, roleMgr)
        {
        }
    }
}