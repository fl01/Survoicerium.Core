using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Survoicerium.Backend.ApiModels;

namespace Survoicerium.Backend.Controllers
{
    public class GameController : Controller
    {
        [HttpPost("api/game")]
        public async Task<IAsyncResult> HandleJoinGameRequest([FromBody]GameInfo gameInfo)
        {
            return Task.FromResult(Ok());
        }
    }
}
