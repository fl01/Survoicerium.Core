using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Survoicerium.Backend.ApiModels;
using Survoicerium.Messaging;
using Survoicerium.Messaging.Events;
using Survoicerium.Messaging.RabbitMq;
using Survoicerium.Messaging.Serialization;

namespace Survoicerium.Backend.Controllers
{
    public class DevController : Controller
    {
        [HttpGet("api/dev")]
        public async Task<IActionResult> TestMessageQueue([FromQuery]string message)
        {
            var config = new MessageQueueConfiguration()
            {
                Host = "localhost",
                Password = "guest",
                User = "guest"
            };

            IEventBus eventBus = new RabbitMqEventBus(config.Host, config.User, config.Password, new JsonSerializer());
            await eventBus.PublishAsync<PingEvent>(new PingEvent() { Message = message ?? "hello" });
            return Ok();
        }
    }
}
