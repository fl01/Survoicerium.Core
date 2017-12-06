using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Survoicerium.InternalConfigurationApiClient.Models;

namespace Survoicerium.InternalConfigurationApiClient.Http
{
    public class RetriableHttpClient : IHttpClient
    {
        private readonly TimeSpan _delayBetweenReties;
        private readonly int _maxRetries;

        public RetriableHttpClient(TimeSpan delayBetweenReties, int maxRetries)
        {
            _delayBetweenReties = delayBetweenReties;
            _maxRetries = maxRetries;
        }

        public async Task<HttpResponse<TResponse>> GetAsync<TResponse>(string requestUri)
            where TResponse : class
        {
            return await TryRequest(async () => await InternalGetAsync<TResponse>(requestUri));
        }

        private async Task<HttpResponse<TResponse>> InternalGetAsync<TResponse>(string requestUri)
            where TResponse : class
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(requestUri);

                var responseBody = await response.Content.ReadAsStringAsync();

                return new HttpResponse<TResponse>()
                {
                    Result = JsonConvert.DeserializeObject<TResponse>(responseBody),
                    StatusCode = response.StatusCode,
                    IsSuccess = response.IsSuccessStatusCode
                };
            }
        }

        private async Task<T> TryRequest<T>(Func<Task<T>> action)
        {
            int retry = 0;
            while (retry++ < _maxRetries)
            {
                try
                {
                    return await action();
                }
                catch (HttpRequestException)
                {
                    //retriable
                }
                catch (Exception)
                {
                    //non-retriable
                    break;
                }

                Thread.Sleep(_delayBetweenReties);
            }

            return default(T);
        }
    }
}
