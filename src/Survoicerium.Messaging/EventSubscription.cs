using System;

namespace Survoicerium.Messaging.RabbitMq
{
    public class EventSubscription
    {
        public Type Type { get; set; }

        public string MessageId { get; set; }

        public Action<object> Handler { get; set; }
    }
}
