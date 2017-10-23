using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Survoicerium.Frontend.Pages
{
    public class GetApiKey : PageModel
    {
        public async void OnGet([FromQuery]string hardwareId)
        {
            var r = Redirect("http://google.com");
            await r.ExecuteResultAsync(PageContext);
        }
    }
}