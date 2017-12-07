using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Survoicerium.Core;
using Survoicerium.Core.Dto;
using Survoicerium.GameApi.ApiModels;

namespace Survoicerium.GameApi.Controllers
{
    public class GameController : Controller
    {
        private readonly IApiUserService _userService;
        private readonly IGameService _gameService;

        public GameController(IApiUserService userService, IGameService gameService)
        {
            _userService = userService;
            _gameService = gameService;
        }

        [HttpPost("api/game")]
        public async Task<IActionResult> HandleJoinGameRequest([FromBody]GameInfo gameInfo)
        {
            var user = await _userService.GetUserByApiKeyAsync(gameInfo.ApiKey);
            if (user == null)
            {
                return NotFound();
            }

            var dto = new GameInfoDto()
            {
                Hash = gameInfo.Hash,
                User = user
            };

            await _gameService.JoinGameAsync(dto);

            return Accepted();
        }
    }
}
