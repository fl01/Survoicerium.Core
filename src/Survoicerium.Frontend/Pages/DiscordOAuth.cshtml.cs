using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Survoicerium.Frontend.Pages
{
    public class DiscordOAuthModel : PageModel
    {
        public string Token { get; set; }

        public void OnGet([FromQuery]string token)
        {
            Token = token;
        }
    }
}