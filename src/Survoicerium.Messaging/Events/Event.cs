using System;

namespace Survoicerium.Messaging.Events
{
    public abstract class Event : Message
    {
        public Event()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }
    }
}
