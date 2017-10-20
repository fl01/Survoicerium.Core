using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Survoicerium.Messaging.Events;

namespace Survoicerium.Messaging
{
    public interface IEventBus
    {
        Task PublishAsync<TMessage>(TMessage body, string messageId = null, [CallerMemberName] string callerName = null)
            where TMessage : Event;
    }
}
