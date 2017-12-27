using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Survoicerium.Core.Abstractions;
using Survoicerium.Core.Dto;
using Survoicerium.GameApi.ApiModels.RequestModels;
using Survoicerium.GameApi.Authorization;

namespace Survoicerium.GameApi.Controllers
{
    public class GameController : BaseController
    {
        private readonly IApiUserService _userService;
        private readonly IGameService _gameService;

        public GameController(IApiUserService userService, IGameService gameService)
        {
            _userService = userService;
            _gameService = gameService;
        }

        [HttpPost("api/game")]
        [Authorize(nameof(ApiKeyRequirement))]
        public async Task<IActionResult> HandleJoinGameRequest([FromBody]GameInfo gameInfo)
        {
            // it is already been authorized by auth filter
            var userApiKey = GetCurrentUserApiKey();

            var user = await _userService.GetUserByApiKeyAsync(userApiKey);
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
