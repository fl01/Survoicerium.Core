using System.Threading.Tasks;
using Survoicerium.InternalConfigurationApiClient.Models;

namespace Survoicerium.InternalConfigurationApiClient.Http
{
    public interface IHttpClient
    {
        Task<HttpResponse<TResponse>> GetAsync<TResponse>(string requestUri)
            where TResponse : class;
    }
}
