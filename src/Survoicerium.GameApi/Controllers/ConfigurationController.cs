using System.Net;
using Microsoft.AspNetCore.Mvc;
using Survoicerium.GameApi.ApiModels;

namespace Survoicerium.GameApi.Controllers
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
