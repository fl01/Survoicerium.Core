using System;
using Survoicerium.InternalConfigurationApiClient.Http;

namespace Survoicerium.InternalConfigurationApiClient
{
    public static class InternalConfigurationApiClientFactory
    {
        public static InternalConfigurationApiClient Create(string internalConfigurationHost, TimeSpan delayBetweenReties, int maxRetries)
        {
            if (string.IsNullOrWhiteSpace(internalConfigurationHost) || !Uri.TryCreate(internalConfigurationHost, UriKind.Absolute, out Uri host))
            {
                throw new ArgumentException($"Invalid {nameof(internalConfigurationHost)} url.");
            }

            return new InternalConfigurationApiClient(new RetriableHttpClient(delayBetweenReties, maxRetries), host);
        }
    }
}
