using System;
using AspNetCore.Health;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Survoicerium.Discord.ApiClient;
using Survoicerium.Discord.ApiClient.Http;
using Survoicerium.Frontend.Configuration;
using Survoicerium.Infrastructure.Mongo;
using Survoicerium.InternalConfigurationApi.Contracts.Models;
using Survoicerium.InternalConfigurationApiClient;

namespace Survoicerium.Frontend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var sysClient = InternalConfigurationApiClientFactory.Create("http://localhost:5100", TimeSpan.FromSeconds(5), 5);
            var sysConfig = sysClient.GetConfigurationAsync().GetAwaiter().GetResult();

            services.AddHealthChecks(context => context.AddUrlCheck("https://google.com"));

            services
                .RegisterApiService("mongodb://localhost:27017")
                .AddScoped<DiscordApiClient>(builder =>
                {
                    return new DiscordApiClient(new SimpleHttpClient(), sysConfig.DiscordOAuth.ClientId, sysConfig.DiscordOAuth.ClientSecret);
                })
                .AddSingleton<DiscordOAuth>(b => Mapper.Map<DiscordOAuthConfiguration, DiscordOAuth>(sysConfig.DiscordOAuth));
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
