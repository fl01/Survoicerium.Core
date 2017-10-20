using System;
using Survoicerium.Discord.Bot.ApiClient.Models;

namespace Survoicerium.Discord.Bot.ApiClient
{
    public class BackendApiClient
    {
        private readonly Guid _id;
        private readonly string _host;

        public BackendApiClient(Guid id, string host)
        {
            _id = id;
            _host = host;
        }

        public MessageQueueConfiguration GetConfiguration()
        {
            return new MessageQueueConfiguration()
            {
                Host = "localhost",
                Password = "guest",
                User = "guest"
            };
        }
    }
}
