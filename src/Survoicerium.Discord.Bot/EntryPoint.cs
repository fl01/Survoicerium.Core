using System;
using System.Threading.Tasks;
using Survoicerium.InternalConfigurationApiClient;
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
            var configClient = InternalConfigurationApiClientFactory.Create("http://localhost:5100", TimeSpan.FromSeconds(5), 5);
            var systemConfig = await configClient.GetConfigurationAsync();

            var eventChannel = new RabbitMqEventChannel(systemConfig.MessageQueue.Host, systemConfig.MessageQueue.User, systemConfig.MessageQueue.Password, new JsonSerializer(), RabbitMqConsts.GenericEventQueueName);
            //eventChannel.Start();
            var svc = new DiscordService(systemConfig.DiscordBot.ApiKey, eventChannel);

            await svc.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
