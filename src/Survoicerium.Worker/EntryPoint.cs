using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Survoicerium.Infrastructure.Mongo;
using Survoicerium.InternalConfigurationApiClient;
using Survoicerium.Messaging;
using Survoicerium.Messaging.RabbitMq;
using Survoicerium.Messaging.Serialization;

namespace Survoicerium.Worker
{
    public class EntryPoint
    {
        public static void Main(string[] args)
            => new EntryPoint().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var configClient = InternalConfigurationApiClientFactory.Create(GetConfiguration().GetValue<string>("System:InternalConfiguration:Host"), TimeSpan.FromSeconds(5), 5);
            var systemConfig = await configClient.GetConfigurationAsync();

            // TODO : ideally queue name should be read from InternalConfigurationApi
            IMessageChannel messageChannel = new RabbitMqChannel(systemConfig.MessageQueue.Host, systemConfig.MessageQueue.User, systemConfig.MessageQueue.Password, new JsonSerializer(), RabbitMqConsts.WorkerQueueName);
            IMessageBus messageBus = new RabbitMqBus(systemConfig.MessageQueue.Host, systemConfig.MessageQueue.User, systemConfig.MessageQueue.Password, new JsonSerializer());
            var worker = new WorkerService(messageChannel, messageBus, new ChannelPersistence(new MongoDbOptions(systemConfig.ChannelsDb.DbHost, systemConfig.ChannelsDb.DbName, systemConfig.ChannelsDb.CollectionName, systemConfig.ChannelsDb.User, systemConfig.ChannelsDb.Password)));
            worker.Start();

            await Task.Delay(-1);
        }

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
