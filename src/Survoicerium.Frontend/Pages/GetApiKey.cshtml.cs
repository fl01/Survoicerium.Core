using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Survoicerium.Frontend.Configuration;

namespace Survoicerium.Frontend.Pages
{
    public class GetApiKey : PageModel
    {
        private readonly DiscordOAuth _discordOAuth;
        private string _hardwareId;

        public ulong DiscordUserId { get; set; } = 0;

        public GetApiKey(IOptions<DiscordOAuth> options)
        {
            _discordOAuth = options.Value;
        }

        public async void OnGet([FromQuery]string hardwareId, [FromQuery]string code)
        {
            if (string.IsNullOrEmpty(_hardwareId) && !string.IsNullOrEmpty(hardwareId))
            {
                _hardwareId = hardwareId;
                var redirect = Redirect(GetDiscordOAuthUrl());
                await redirect.ExecuteResultAsync(PageContext);
            }
            else if (!string.IsNullOrEmpty(code))
            {
                DiscordUserId = 42;
            }
        }

        private string GetDiscordOAuthUrl()
        {
            // example https://discordapp.com/api/oauth2/authorize?response_type=code&client_id=157730590492196864&scope=identify%20guilds.join&state=15773059ghq9183habn&redirect_uri=https%3A%2F%2Fnicememe.website
            return $"https://discordapp.com/api/oauth2/authorize?response_type=code&client_id={_discordOAuth.ClientId}&scope={_discordOAuth.Scope}&redirect_uri={_discordOAuth.RedirectUrl}";
        }
    }
}
