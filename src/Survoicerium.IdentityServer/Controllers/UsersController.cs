using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Survoicerium.IdentityServer.Controllers
{
    public class ValuesController : Controller
    {
        [HttpGet("api/me")]
        public IEnumerable<string> GetMe()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
