using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Survoicerium.Discord.ApiClient.Http
{
    public class SimpleHttpClient
    {
        public async Task<TResponse> PostAsync<TResponse>(string requestUri, IEnumerable<KeyValuePair<string, string>> data)
        {
            using (var httpClient = new HttpClient())
            {
                var body = new FormUrlEncodedContent(data)
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                    }
                };
                var result = await httpClient.PostAsync(requestUri, body);
                var responseBody = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseBody);
            }
        }

        public async Task<TResponse> GetAsync<TResponse>(string requestUri, AccessTokenResponse token = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (token != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token.TokenType, token.AccessToken);
                }

                var result = await httpClient.GetAsync(requestUri);
                var responseBody = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseBody);
            }
        }
    }
}
