using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Survoicerium.Core;
using Survoicerium.GameApi.ApiModels;

namespace Survoicerium.GameApi.Controllers
{
    public class GameController : Controller
    {
        private readonly IApiUserService _userService;

        public GameController(IApiUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("api/game")]
        public async Task<IAsyncResult> HandleJoinGameRequest([FromBody]GameInfo gameInfo)
        {
            return Task.FromResult(Ok());
        }
    }
}
