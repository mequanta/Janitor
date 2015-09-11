using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Linq;
using System;
namespace Api1
{
    [Route("claims")]
    public class ClaimsController : Controller
    {
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
			Console.WriteLine(User);
            var claims = from c in User.Claims
                         select new { c.Type, c.Value };
			Console.WriteLine(claims);
            return Json(claims);
        }
    }
}