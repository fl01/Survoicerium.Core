using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Survoicerium.IdentityServer.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        [HttpGet("api/me")]
        public IActionResult GetMe()
        {
            return Ok();
        }
    }
}
