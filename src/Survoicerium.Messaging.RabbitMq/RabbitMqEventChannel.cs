using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Survoicerium.Messaging.Events;
using Survoicerium.Messaging.Serialization;

namespace Survoicerium.Messaging.RabbitMq
{
    public class RabbitMqEventChannel : RabbitMqQueueSubscriber, IEventChannel
    {
        private List<EventSubscription> _subs = new List<EventSubscription>();
        private readonly IMessageSerializer _serializer;

        public RabbitMqEventChannel(string host, string user, string password, IMessageSerializer serializer, string queueName)
            : base(host, user, password, serializer, queueName)
        {
            _serializer = serializer;
        }

        public void On<T>(Action<object> handler)
            where T : Event
        {
            var sub = new EventSubscription()
            {
                Type = typeof(T),
                MessageId = typeof(T).Name,
                Handler = handler
            };

            _subs.Add(sub);
        }

        protected override bool ProcessMessage(string messageId, byte[] body)
        {
            var handlers = _subs.Where(s => string.Equals(s.MessageId, messageId, StringComparison.OrdinalIgnoreCase));
            if (!handlers.Any())
            {
                // TODO : logger
                Console.WriteLine($"Event '{messageId}' is unhandled.");
                return false;
            }

            foreach (var sub in handlers)
            {
                Task.Factory.StartNew(() =>
                {
                    object args = null;
                    try
                    {
                        args = _serializer.Deserialize(body, sub.Type);
                    }
                    catch (Exception ex)
                    {
                        // TODO : logger
                        Console.WriteLine($"Failed to serialize body of event '{messageId}'. Error {ex.ToString()}");
                    }

                    try
                    {
                        sub.Handler(args);
                    }
                    catch (Exception ex)
                    {
                        // TODO : logger
                        Console.WriteLine($"Failed to handle '{messageId}'. Error {ex.ToString()}");
                    }
                });
            }

            return true;
        }
    }
}
