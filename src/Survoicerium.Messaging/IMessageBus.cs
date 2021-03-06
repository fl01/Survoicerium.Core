﻿using System.Threading.Tasks;
using Survoicerium.Messaging.Messages;

namespace Survoicerium.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync<TMessage>(TMessage body)
            where TMessage : Message;
    }
}
