using AspNetCore.Health;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Survoicerium.Discord.ApiClient;
using Survoicerium.Discord.ApiClient.Http;
using Survoicerium.Frontend.Configuration;
using Survoicerium.Infrastructure.Mongo;

namespace Survoicerium.Frontend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks(context => context.AddUrlCheck("https://google.com"));

            services
                .RegisterApiService("mongodb://localhost:27017")
                .AddScoped<DiscordApiClient>(builder =>
                {
                    var settings = builder.GetRequiredService<IOptions<DiscordOAuth>>().Value;
                    return new DiscordApiClient(new SimpleHttpClient(), settings.ClientId, settings.ClientSecret);
                })
                .Configure<DiscordOAuth>(Configuration.GetSection(nameof(DiscordOAuth)));

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseHealthCheck("/health");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
