using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Configuration;
using Microsoft.Dnx.Runtime;

namespace Janitor.Test.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath);
            builder.AddJsonFile("config.json",optional: true);
            builder.AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();
            app.UseStaticFiles();
            var authority = Configuration["IDP_AUTHORITY"] ?? "http://localhost:44002";
            var audience = $"{authority}/resources";
            app.UseOAuthBearerAuthentication(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.AutomaticAuthentication = true;
            });

            app.UseMiddleware<RequiredScopesMiddleware>(new List<string> { "testapi" });
            app.UseMvc();
            // Add the following route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
        }
    }
}
