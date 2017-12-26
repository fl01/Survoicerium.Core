using System;
using Survoicerium.Messaging.Events;

namespace Survoicerium.Messaging
{
    public interface IMessageChannel
    {
        void On<T>(Action<object> handler)
            where T : Message;

        void Start();
    }
}
