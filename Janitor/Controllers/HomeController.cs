using IdentityServer3.Core.Logging;
using Microsoft.Owin;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Text;

namespace Janitor
{
    class HomeActionResult : IHttpActionResult
    {
        readonly IOwinContext context;

        public HomeActionResult(IOwinContext context)
        {
            this.context = context;
        }

        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            var content = new StringContent("sss", Encoding.UTF8, "text/html");

            var response = new HttpResponseMessage
            {
                Content = content
            };

            return Task.FromResult(response);
        }
    }

    class HomeController : ApiController
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();

        public HomeController()
        {
        }

        public IHttpActionResult Get()
        {
            Logger.Info("Home page requested - rendering");
            return new HomeActionResult(Request.GetOwinContext());
        }
    }
}
