using Owin;
using Janitor.IdSvr;
using IdentityManager;
using IdentityManager.Configuration;
using IdentityServer3.Core.Configuration;
using Janitor.IdMgr;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Twitter;
using IdentityManager.Core.Logging;
using Serilog;
using IdentityManager.Logging;
using Owin.Security.AesDataProtectorProvider;
using IdentityServer3.Core.Services;
using System.Web.Http;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin;
using System.IO;
using System;

namespace Janitor
{
    internal class Startup
    {
        readonly string basePath;
        public Startup(string basePath)
        {
            this.basePath = basePath;
        }

        public void Configuration(IAppBuilder app)
        {
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Trace()
               .CreateLogger();

            app.UseAesDataProtectorProvider();
            app.Map("/admin", adminApp =>
                {
                    var factory = new IdentityManagerServiceFactory();
                   
                    factory.ConfigureSimpleIdentityManagerService("AspId");
                    //factory.ConfigureCustomIdentityManagerServiceWithIntKeys("AspId_CustomPK");

                    var adminOptions = new IdentityManagerOptions(){
                        Factory = factory
                    };
                    adminOptions.SecurityConfiguration.RequireSsl = false;
                    adminApp.UseIdentityManager(adminOptions);
                });

            var idSvrFactory = Factory.Configure();
            idSvrFactory.ConfigureUserService("AspId");

            var viewOptions = new ViewServiceOptions
            {
                TemplatePath = this.basePath.TrimEnd(new char[] { '/' })
            };
            idSvrFactory.ViewService = new IdentityServer3.Core.Configuration.Registration<IViewService>(new ViewService(viewOptions));

            var options = new IdentityServerOptions
            {
                SiteName = "IdentityServer3 - ViewSerive-AspNetIdentity",
                SigningCertificate = Certificate.Get(),
                Factory = idSvrFactory,
                RequireSsl = false,
                AuthenticationOptions = new AuthenticationOptions
                {
                    IdentityProviders = ConfigureAdditionalIdentityProviders,
                }
            };

            app.Map("/core", core =>
            {
                core.UseIdentityServer(options);
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/Content"),
                FileSystem = new PhysicalFileSystem(Path.Combine(this.basePath, "Content")) 
            });

            var config = new HttpConfiguration();
          //  config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("API", "api/{controller}/{action}", new { controller = "Home", action = "Get" });
            app.UseWebApi(config);
        }

        public static void ConfigureAdditionalIdentityProviders(IAppBuilder app, string signInAsType)
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
    }
}