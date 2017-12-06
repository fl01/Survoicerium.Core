using System.Net;

namespace Survoicerium.InternalConfigurationApiClient.Models
{
    public class HttpResponse<T>
    {
        public T Result { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; }
    }
}
