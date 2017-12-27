using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Survoicerium.Core;
using Survoicerium.Core.Abstractions;
using Survoicerium.GameApi.ApiModels.ResponseModels;
using Survoicerium.GameApi.Authorization;

namespace Survoicerium.GameApi.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IApiUserService _userService;

        public UsersController(IApiUserService apiUserService)
        {
            _userService = apiUserService;
        }

        [HttpGet("api/me")]
        [Authorize(nameof(ApiKeyRequirement))]
        public async Task<IActionResult> GetMe()
        {
            var apiKey = GetCurrentUserApiKey();

            var user = await _userService.GetUserByApiKeyAsync(apiKey);

            return Ok(Mapper.Map<ApiUser, User>(user));
        }
    }
}
