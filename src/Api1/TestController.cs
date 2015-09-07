using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Security.Claims;
using System;

namespace Api1
{
    [Route("test")]
    public class TestController : Controller
    {
        [Authorize]
        public IActionResult Get()
        {
            Console.WriteLine("hhh");
            var caller = User as ClaimsPrincipal;

            var subjectClaim = caller.FindFirst("sub");
            IActionResult r;
            if (subjectClaim != null)
            {
                r = Json(new
                {
                    message = "OK user",
                    client = caller.FindFirst("client_id").Value,
                    subject = subjectClaim.Value
                });
            }
            else
            {
                r = Json(new
                {
                    message = "OK computer",
                    client = caller.FindFirst("client_id").Value
                });
            }

            return r;
        }
    }
}
