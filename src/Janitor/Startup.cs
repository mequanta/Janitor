using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer3.Core.Configuration;
using Microsoft.Framework.Runtime;
using System.IO;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Microsoft.Owin.Security.Twitter;
using Owin;
using Owin.Security.Providers.GitHub;
using Owin.Security.AesDataProtectorProvider;
using Serilog;

namespace IdentityServerAspNet5
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
        }

        public void Configure(IApplicationBuilder app, IApplicationEnvironment env)
        {
            var certFile = Path.Combine(env.ApplicationBasePath, "idsrv3test.pfx");
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Trace().CreateLogger();

            var idsrvOptions = new IdentityServerOptions
            {
                Factory = new IdentityServerServiceFactory().UseInMemoryUsers(Users.Get()).UseInMemoryClients(Clients.Get()).UseInMemoryScopes(Scopes.Get()),
                SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                AuthenticationOptions = new AuthenticationOptions
                {
                    IdentityProviders = ConfigureAdditionalIdentityProviders,
                }
            };

            app.UseIdentityServer(idsrvOptions);
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
    }
}