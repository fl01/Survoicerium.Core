using System;

namespace Survoicerium.Messaging.Messages.Events
{
    public abstract class Event : Message
    {
        public Event()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }
    }
}
