using System;
using System.Threading.Tasks;
using Survoicerium.InternalConfigurationApiClient.Http;
using Survoicerium.InternalConfigurationApiClient.Models;

namespace Survoicerium.InternalConfigurationApiClient
{
    public class InternalConfigurationApiClient
    {
        private readonly IHttpClient _httpClient;
        private readonly Uri _host;

        private Uri InternalConfigurationHost { get; set; }

        internal InternalConfigurationApiClient(IHttpClient httpClient, Uri host)
        {
            _httpClient = httpClient;
            _host = host;
        }

        public async Task<ConfigurationContainer> GetConfigurationAsync()
        {
            var response = await _httpClient.GetAsync<ConfigurationContainer>($"{_host.AbsoluteUri}api/cfg");

            return response?.Result;
        }
    }
}
