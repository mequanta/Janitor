using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Linq;

namespace Janitor.Test.Api.Controllers
{
    [Route("api/[controller]")]
    public class ClaimsController : Controller
    {
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            var claims = from c in User.Claims
                         select new { c.Type, c.Value };
            return Json(claims);
        }
    }
}