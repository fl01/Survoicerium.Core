using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Survoicerium.Discord.ApiClient.Http;

namespace Survoicerium.Discord.ApiClient
{
    public class DiscordApiClient
    {
        private readonly SimpleHttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public const string TokenUri = "oauth2/token";
        public const string MeUri = "users/@me";
        public const string ApiEndpoint = "https://discordapp.com/api/v6";

        public DiscordApiClient(SimpleHttpClient httpClient, string clientId, string clientSecret)
        {
            _httpClient = httpClient;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<ExchangeCodeResult> ExchangeCodeAsync(string code, string redirectUri)
        {
            var parameters = BuildExchangeCodeParameters(code, redirectUri);
            var token = await _httpClient.PostAsync<AccessTokenResponse>($"{ApiEndpoint}/{TokenUri}", parameters);
            if (string.IsNullOrEmpty(token.AccessToken))
            {
                return null;
            }

            var user = await _httpClient.GetAsync<UserInfo>($"{ApiEndpoint}/{MeUri}", token);

            return new ExchangeCodeResult()
            {
                Token = token,
                User = user
            };
        }

        private Dictionary<string, string> BuildExchangeCodeParameters(string code, string redirectUri)
        {
            return new Dictionary<string, string>()
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "redirect_uri", "http://localhost:5001/getapikey" },
                { "code", code },
                { "grant_type", "authorization_code" }
            };
        }
    }
}
