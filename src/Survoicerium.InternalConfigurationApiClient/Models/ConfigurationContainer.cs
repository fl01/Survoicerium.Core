namespace Survoicerium.InternalConfigurationApiClient.Models
{
    public class ConfigurationContainer
    {
        public DiscordBotConfiguration DiscordBot { get; set; }

        public MessageQueueConfiguration MessageQueue { get; set; }

        public BackendClientConfiguration BackendClient { get; set; }
    }
}
