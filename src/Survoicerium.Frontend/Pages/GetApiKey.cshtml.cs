using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Survoicerium.Core;
using Survoicerium.Core.Dto;
using Survoicerium.Discord.ApiClient;
using Survoicerium.Frontend.Configuration;

namespace Survoicerium.Frontend.Pages
{
    public class GetApiKey : PageModel
    {
        private readonly DiscordOAuth _discordOAuth;
        private readonly IApiUserService _apiUserService;
        private readonly DiscordApiClient _apiClient;

        public string ApiKey { get; set; }

        public string ErrorMessage { get; set; }

        private string EncodedRedirectUrl
        {
            get
            {
                return HttpUtility.UrlEncode(_discordOAuth.RedirectUrl);
            }
        }

        public GetApiKey(DiscordOAuth options, IApiUserService apiUserService, DiscordApiClient apiClient)
        {
            _discordOAuth = options;
            _apiUserService = apiUserService;
            _apiClient = apiClient;
        }

        public async void OnGet([FromQuery]string code, [FromQuery]string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                ErrorMessage = "Something went wrong. Please try again later";
                return;
            }

            IApiUser apiUser = _apiUserService.GetUserByHardwareIdAsync(state).GetAwaiter().GetResult();
            if (apiUser != null)
            {
                ApiKey = apiUser.ApiKey;
                return;
            }

            else if (string.IsNullOrEmpty(code))
            {
                await BeginNewKeyCreation(state);
                return;
            }
            else
            {
                // we need to wait in order to get correct render
                CompleteUserCreation(code, state).GetAwaiter().GetResult();
                return;
            }
        }

        private async Task CompleteUserCreation(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                ErrorMessage = "Invalid external code. Please try again later";
            }

            // redirect url should not be encoded
            var data = await _apiClient.ExchangeCodeAsync(code, _discordOAuth.RedirectUrl);
            if (data?.User == null || data.User.Id == 0)
            {
                ErrorMessage = "Failed to communicate with Discord servers. Please try again later";
                return;
            }
            else
            {
                var dto = new AddUserDto()
                {
                    Code = code,
                    HardwareId = state,
                    DiscordUserId = data.User.Id
                };

                IApiUser apiUser = await _apiUserService.AddAsync(dto);
                ApiKey = apiUser.ApiKey;
            }
        }

        private async Task BeginNewKeyCreation(string state)
        {
            var redirect = Redirect(GetDiscordOAuthUrl(state));
            await redirect.ExecuteResultAsync(PageContext);
        }

        private string GetDiscordOAuthUrl(string state)
        {
            // example https://discordapp.com/api/oauth2/authorize?response_type=code&client_id=157730590492196864&scope=identify%20guilds.join&state=15773059ghq9183habn&redirect_uri=https%3A%2F%2Fnicememe.website
            return $"https://discordapp.com/api/oauth2/authorize?response_type=code&client_id={_discordOAuth.ClientId}&scope={_discordOAuth.Scope}&redirect_uri={EncodedRedirectUrl}&state={state}";
        }
    }
}
