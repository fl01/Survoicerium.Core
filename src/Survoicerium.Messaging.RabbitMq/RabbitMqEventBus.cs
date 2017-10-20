using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Survoicerium.Messaging.Events;
using Survoicerium.Messaging.Serialization;

namespace Survoicerium.Messaging.RabbitMq
{
    public class RabbitMqEventBus : RabbitMqExchangeSender<Event>, IEventBus
    {
        protected override string ExchangeName { get; } = RabbitMqConsts.EventsExchangeName;

        protected override string RoutingKey { get; } = RabbitMqConsts.EventsRoutingKey;

        public RabbitMqEventBus(string rabbitHostName, string rabbitLogin, string rabbitPassword, IMessageSerializer serializer)
            : base(rabbitHostName, rabbitLogin, rabbitPassword, serializer)
        {
        }

        public Task PublishAsync<TMessage>(TMessage body, string messageId = null, [CallerMemberName]string callerName = "")
            where TMessage : Event
        {
            if (messageId == null)
            {
                messageId = typeof(TMessage).Name;
            }

            return SendInternalAsync(body, messageId, callerName);
        }
    }
}
