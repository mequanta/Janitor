using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using System.IdentityModel.Tokens;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Framework.Configuration;
using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;

namespace Janitor.Test.MvcClient
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath);
            builder.AddJsonFile("config.json");
            builder.AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);
            builder.AddUserSecrets();
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(options =>
            {
                options.AuthenticationScheme = "Cookies";
                options.AutomaticAuthentication = true;
            });

            var authority = Configuration["IDP_AUTHORITY"] ?? "http://localhost:44002";
            var audience = $"{authority}/resources";
            var redirectUri = Configuration["REDIRECT_URI"] ?? "http://localhost:2221/";
            app.UseOpenIdConnectAuthentication(options =>
            {
                options.Authority = authority;
                options.ClientId = "mvc6";
                options.ResponseType = "id_token token";
                options.Scope = "openid email profile testapi";
                options.RedirectUri = redirectUri;
                options.SignInScheme = "Cookies";
                options.AutomaticAuthentication = true;

                options.Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = n =>
                        {
                    var incoming = n.AuthenticationTicket.Principal;

                        // create application identity
                        var id = new ClaimsIdentity("application", "given_name", "role");
                    id.AddClaim(incoming.FindFirst("sub"));
                    id.AddClaim(incoming.FindFirst("email"));
                    id.AddClaim(incoming.FindFirst("email_verified"));
                    id.AddClaim(incoming.FindFirst("given_name"));
                    id.AddClaim(incoming.FindFirst("family_name"));
                    id.AddClaim(new Claim("token", n.ProtocolMessage.AccessToken));

                    n.AuthenticationTicket = new AuthenticationTicket(
                        new ClaimsPrincipal(id),
                        n.AuthenticationTicket.Properties,
                        n.AuthenticationTicket.AuthenticationScheme);

                        // this skips nonce checking & cleanup 
                        // see https://github.com/aspnet/Security/issues/372
                        n.HandleResponse();
                    return Task.FromResult(0);
                }
                };
            });

            app.UseMvcWithDefaultRoute();
        }
    }
}