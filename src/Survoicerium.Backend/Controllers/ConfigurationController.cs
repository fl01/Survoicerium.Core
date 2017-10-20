using System.Net;
using Microsoft.AspNetCore.Mvc;
using Survoicerium.Backend.ApiModels;

namespace Survoicerium.Backend.Controllers
{
    public class ConfigurationController : Controller
    {
        [HttpGet("api/configuration")]
        [ProducesResponseType(typeof(MessageQueueConfiguration), (int)HttpStatusCode.OK)]
        public IActionResult GetConfiguration()
        {
            return Ok(new MessageQueueConfiguration()
            {
                Host = "localhost",
                Password = "guest",
                User = "guest"
            });
        }
    }
}
