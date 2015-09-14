using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using IdentityManager.Configuration;
using System.Threading.Tasks;

#if DNX451 || DNXCORE50
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Dnx.Runtime;
using Microsoft.AspNet.DataProtection;
#else

#endif
using IdentityServer3.Core.Configuration;
using IdentityServer3.WsFederation.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Services.InMemory;
using IdentityServer3.WsFederation.Models;
using IdentityServer3.WsFederation.Services;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Microsoft.Owin.Security.Twitter;
using Owin;
using Owin.Security.Providers.GitHub;
using Owin.Security.AesDataProtectorProvider;
using Serilog;

namespace Janitor
{
    using DataProtectionProviderDelegate = Func<string[], Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>>;
    using DataProtectionTuple = Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>;
    public partial class Startup
    {
        public static string BasePath = null;
#if DNX451 || DNXCORE50
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
        }

        public void Configure(IApplicationBuilder app, IApplicationEnvironment env)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Trace().CreateLogger();
            BasePath = env.ApplicationBasePath;
            var certFile = Path.Combine(BasePath, "idsrv3test.pfx");
            var options = ConfigureIdentityServer(certFile);
          
            app.UseOwin(pipeline =>
            {
                pipeline(next =>
                {
                    var builder = new Microsoft.Owin.Builder.AppBuilder();
                    var provider = app.ApplicationServices.GetService<Microsoft.AspNet.DataProtection.IDataProtectionProvider>();

                    builder.Properties["security.DataProtectionProvider"] = new DataProtectionProviderDelegate(purposes =>
                    {
                        var dataProtection = provider.CreateProtector(String.Join(",", purposes));
                        return new DataProtectionTuple(dataProtection.Protect, dataProtection.Unprotect);
                    });

                    builder.Map("/admin", adminApp =>
                    {
                        var factory = new IdentityManagerServiceFactory();
                        factory.ConfigureSimpleIdentityManagerService("AspId");
                        //factory.ConfigureCustomIdentityManagerServiceWithIntKeys("AspId_CustomPK");

                        adminApp.UseIdentityManager(new IdentityManagerOptions()
                        {
                            Factory = factory
                        });
                    });

		            builder.UseAesDataProtectorProvider();
                    builder.UseIdentityServer(options);
                    var appFunc = builder.Build(typeof(Func<IDictionary<string, object>, Task>)) as Func<IDictionary<string, object>, Task>;
                    return appFunc;
                });
            });

            app.UseIdentityServer(options);
        }
#else
        public static void Configure(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                       .MinimumLevel.Debug()
                       .WriteTo.Trace()
                       .CreateLogger();
            BasePath = AppDomain.CurrentDomain.BaseDirectory;
            var certFile = Path.Combine(BasePath, "idsrv3test.pfx");
            Console.WriteLine(certFile);
            var options = ConfigureIdentityServer(certFile);

            //  var cpath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"..", "..", "..", "src", "Janitor", "Content"));
            //  Console.WriteLine(cpath);
            //  app.UseStaticFiles (new StaticFileOptions {
            //		RequestPath = new PathString("/Content"),
            //		FileSystem = new PhysicalFileSystem(cpath)
            //	});
            app.Map("/admin", adminApp =>
            {
                var factory = new IdentityManagerServiceFactory();
                factory.ConfigureSimpleIdentityManagerService("AspId");
                //factory.ConfigureCustomIdentityManagerServiceWithIntKeys("AspId_CustomPK");

                adminApp.UseIdentityManager(new IdentityManagerOptions()
                {
                    Factory = factory
                });
            });

            app.UseAesDataProtectorProvider();
            app.UseIdentityServer(options);
        }

        public void Configuration(IAppBuilder app)
        {
            Configure(app);
        }
#endif

        private static IdentityServerOptions ConfigureIdentityServer(string certFile)
        {
            //var certFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "idsrv3test.pfx");
            var factory =
                new IdentityServerServiceFactory().UseInMemoryUsers(Users.Get())
                    .UseInMemoryClients(Clients.Get())
                    .UseInMemoryScopes(Scopes.Get());

            var viewOptions = new DefaultViewServiceOptions();
            viewOptions.Stylesheets.Add("/Content/Site.css");
            viewOptions.CacheViews = false;
            factory.ConfigureDefaultViewService(viewOptions);
            factory.ViewService = new IdentityServer3.Core.Configuration.Registration<IViewService>(typeof(CustomViewService));

            //var userService = new LocalRegistrationUserService();
            //factory.UserService = new Registration<IUserService>(resolver => userService);
            //            factory.UserService = new Registration<IUserService, UserService>();

         //   factory.ClaimsProvider = new IdentityServer3.Core.Configuration.Registration<IClaimsProvider>(typeof(CustomClaimsProvider));
         //   factory.UserService = new IdentityServer3.Core.Configuration.Registration<IUserService>(typeof(CustomUserService));
        //    factory.CustomGrantValidators.Add(new IdentityServer3.Core.Configuration.Registration<ICustomGrantValidator>(typeof(CustomGrantValidator)));
            factory.CorsPolicyService = new IdentityServer3.Core.Configuration.Registration<ICorsPolicyService>(new DefaultCorsPolicyService() { AllowAll = true });
            var options = new IdentityServerOptions
            {
                RequireSsl = false,
                SiteName = "Janitor - Mequanta Identity Service",
                Factory = factory,
                SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                AuthenticationOptions = new AuthenticationOptions
                {
                    IdentityProviders = ConfigureIdentityProviders,
                    LoginPageLinks = new LoginPageLink[]
                    {
                        new LoginPageLink()
                        {
                            Text = "Register",
                            Href = "localregistration"
                        }
                    }
                },
                PluginConfiguration = ConfigurePlugins,
                EventsOptions = new EventsOptions()
                {
                    RaiseSuccessEvents = true,
                    RaiseErrorEvents = true,
                    RaiseFailureEvents = true,
                    RaiseInformationEvents = true
                }
            };
            return options;
        }

        private static void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            var google = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                SignInAsAuthenticationType = signInAsType,
                ClientId = "767400843187-8boio83mb57ruogr9af9ut09fkg56b27.apps.googleusercontent.com",
                ClientSecret = "5fWcBT0udKY7_b6E3gEiJlze"
            };
            app.UseGoogleAuthentication(google);

            var fb = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "676607329068058",
                AppSecret = "9d6ab75f921942e61fb43a9b1fc25c63"
            };
            app.UseFacebookAuthentication(fb);

            var twitter = new TwitterAuthenticationOptions
            {
                AuthenticationType = "Twitter",
                SignInAsAuthenticationType = signInAsType,
                ConsumerKey = "N8r8w7PIepwtZZwtH066kMlmq",
                ConsumerSecret = "df15L2x6kNI50E4PYcHS0ImBQlcGIt6huET8gQN41VFpUCwNjM"
            };
            app.UseTwitterAuthentication(twitter);

            var ms = new MicrosoftAccountAuthenticationOptions
            {
                AuthenticationType = "Microsoft",
                SignInAsAuthenticationType = signInAsType,
                ClientId = "N8r8w7PIe",
                ClientSecret = "df15L2x6kNI50E"
            };
            app.UseMicrosoftAccountAuthentication(ms);

            var github = new GitHubAuthenticationOptions()
            {
                AuthenticationType = "Github",
                SignInAsAuthenticationType = signInAsType,
                ClientId = "N8r8w7PIe",
                ClientSecret = "df15L2x6kNI50E"
            };
            app.UseGitHubAuthentication(github);
        }

        private static void ConfigureAdditionalIdentityProviders(IAppBuilder app, string signInAsType)
        {
            var google = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                SignInAsAuthenticationType = signInAsType,
                ClientId = "767400843187-8boio83mb57ruogr9af9ut09fkg56b27.apps.googleusercontent.com",
                ClientSecret = "5fWcBT0udKY7_b6E3gEiJlze"
            };
            app.UseGoogleAuthentication(google);

            var fb = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "676607329068058",
                AppSecret = "9d6ab75f921942e61fb43a9b1fc25c63"
            };
            app.UseFacebookAuthentication(fb);

            var twitter = new TwitterAuthenticationOptions
            {
                AuthenticationType = "Twitter",
                SignInAsAuthenticationType = signInAsType,
                ConsumerKey = "N8r8w7PIepwtZZwtH066kMlmq",
                ConsumerSecret = "df15L2x6kNI50E4PYcHS0ImBQlcGIt6huET8gQN41VFpUCwNjM"
            };
            app.UseTwitterAuthentication(twitter);
        }

        private static void ConfigurePlugins(IAppBuilder pluginApp, IdentityServerOptions options)
        {
            var wfOptions = new WsFederationPluginOptions(options);
            wfOptions.Factory.Register(new IdentityServer3.Core.Configuration.Registration<IEnumerable<RelyingParty>>(RelyingParties.Get()));
            wfOptions.Factory.RelyingPartyService = new IdentityServer3.Core.Configuration.Registration<IRelyingPartyService>(typeof(InMemoryRelyingPartyService));
            pluginApp.UseWsFederationPlugin(wfOptions);
        }
    }
}