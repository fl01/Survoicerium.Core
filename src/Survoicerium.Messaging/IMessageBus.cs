using System.Threading.Tasks;
using Survoicerium.Messaging.Events;

namespace Survoicerium.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync<TMessage>(TMessage body)
            where TMessage : Message;
    }
}
