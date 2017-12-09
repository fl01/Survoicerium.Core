using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Health;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Survoicerium.Core;
using Survoicerium.Core.Hash;
using Survoicerium.GameApi.Authorization;
using Survoicerium.GameApi.Filters;
using Survoicerium.Infrastructure.Mongo;
using Survoicerium.InternalConfigurationApiClient;
using Survoicerium.Messaging;
using Survoicerium.Messaging.RabbitMq;
using Survoicerium.Messaging.Serialization;

namespace Survoicerium.GameApi
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
            var sysClient = InternalConfigurationApiClientFactory.Create(Configuration.GetValue<string>("System:InternalConfiguration:Host"), TimeSpan.FromSeconds(5), 5);
            var sysConfig = sysClient.GetConfigurationAsync().GetAwaiter().GetResult();

            // https://stackoverflow.com/a/45955658/8030072
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.Headers["Location"] = context.RedirectUri;
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.CompletedTask;
                    };
                });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(nameof(ApiKeyRequirement), policy => policy.Requirements.Add(new ApiKeyRequirement()));
            });

            var nameService = new NameService();
            var channel = new RabbitMqEventChannel(sysConfig.MessageQueue.Host, sysConfig.MessageQueue.User, sysConfig.MessageQueue.Password, new JsonSerializer(), RabbitMqConsts.GenericEventQueueName);
            var bus = new RabbitMqEventBus(sysConfig.MessageQueue.Host, sysConfig.MessageQueue.User, sysConfig.MessageQueue.Password, new JsonSerializer());
            var gameService = new GameService(nameService, bus, channel);
            channel.Start();
            services
                .RegisterApiService(sysConfig.UsersDb.ConnectionString, sysConfig.UsersDb.DbName, sysConfig.UsersDb.CollectionName)
                .AddSingleton<IGameService>(gameService)
                .AddScoped<IAuthorizationHandler, ApiKeyHandler>();

            services.AddHealthChecks(context => context.AddUrlCheck("https://google.com"));
            services.AddMvc(opt =>
            {
                opt.Filters.Add<ModelStateValidationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthCheck("/health");
            app.UseMvc();
        }
    }
}
