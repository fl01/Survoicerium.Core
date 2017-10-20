using System;
using Survoicerium.Messaging.Events;

namespace Survoicerium.Messaging
{
    public interface IEventChannel
    {
        void On<T>(Action<object> handler)
            where T : Event;
    }
}
