using Survoicerium.Messaging.Events;

namespace Survoicerium.Messaging
{
    public interface IEventHandler<T> where T : Event
    {
    }
}
