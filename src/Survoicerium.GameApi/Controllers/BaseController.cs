using Microsoft.AspNetCore.Mvc;
using Survoicerium.GameApi.Authorization;

namespace Survoicerium.GameApi.Controllers
{
    public class BaseController : Controller
    {
        protected string GetCurrentUserApiKey()
        {
            return HttpContext.Request.Headers[ApiKeyHandler.ApiKeyHeader][0];
        }
    }
}
