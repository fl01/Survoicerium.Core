using System;
using Survoicerium.Messaging.Messages;

namespace Survoicerium.Messaging
{
    public interface IMessageChannel
    {
        void On<TMessage>(Action<object> handler)
            where TMessage : Message;

        void Start();
    }
}
