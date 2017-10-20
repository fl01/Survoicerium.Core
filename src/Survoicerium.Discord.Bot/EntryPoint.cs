using System;
using System.Threading.Tasks;
using Survoicerium.Discord.Bot.ApiClient;
using Survoicerium.Messaging.RabbitMq;
using Survoicerium.Messaging.Serialization;

namespace Survoicerium.Discord.Bot
{
    public class EntryPoint
    {
        public static void Main(string[] args)
            => new EntryPoint().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var apiClient = new BackendApiClient(Guid.NewGuid(), "http://localhost");
            var config = apiClient.GetConfiguration();
            var eventChannel = new RabbitMqEventChannel(config.Host, config.User, config.Password, new JsonSerializer(), RabbitMqConsts.GenericEventQueueName);
            var svc = new DiscordService("MzcwOTg4NTM1NDI1MjY5NzYy.DMvJCQ.ZTz5a5kRdOtMshSXROJrPiAb-Ec", apiClient, eventChannel);

            await svc.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
