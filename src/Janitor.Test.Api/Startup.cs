using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNet.Authentication.OAuthBearer;
namespace Janitor.Test.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            app.UseStaticFiles();
            app.UseOAuthBearerAuthentication(options =>
            {
                options.Authority = "https://localhost:44333";
                options.Audience = "https://localhost:44333/resources";                
                options.AutomaticAuthentication = true;
            });

            app.UseMiddleware<RequiredScopesMiddleware>(new List<string> { "testapi" });
            app.UseMvc();
            // Add the following route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
        }
    }
}
