using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Survoicerium.IdentityServer
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:5002")
                .Build();
    }
}
