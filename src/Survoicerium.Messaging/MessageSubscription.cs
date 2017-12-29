using System;

namespace Survoicerium.Messaging
{
    public class MessageSubscription
    {
        public Type Type { get; set; }

        public string MessageId { get; set; }

        public Action<object> Handler { get; set; }
    }
}
