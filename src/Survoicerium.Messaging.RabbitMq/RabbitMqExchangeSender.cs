using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Survoicerium.Messaging.Serialization;

namespace Survoicerium.Messaging.RabbitMq
{
    public abstract class RabbitMqExchangeSender<T>
    {
        private IModel _channel;
        private readonly IMessageSerializer _serializer;

        protected abstract string ExchangeName { get; }
        protected abstract string RoutingKey { get; }

        protected RabbitMqExchangeSender(string rabbitHostName, string rabbitLogin, string rabbitPassword, IMessageSerializer serializer)
        {
            _serializer = serializer;

            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitHostName,
                Password = rabbitPassword,
                UserName = rabbitLogin,

                RequestedHeartbeat = 10,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(1.0)
            };

            var connection = connectionFactory.CreateConnection();
            connection.ConnectionShutdown += ConnectionShutdown;
            _channel = connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, false);
        }

        protected void SendInternal(T body, string messageId, [CallerMemberName]string callerName = "")
        {
            try
            {
                byte[] data = _serializer.Serialize(body);
                var properties = new RabbitMQ.Client.Framing.BasicProperties()
                {
                    MessageId = messageId
                };

                _channel.BasicPublish(ExchangeName, RoutingKey, properties, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected Task SendInternalAsync(T body, string messageId, [CallerMemberName]string callerName = "")
        {
            return Task.Factory.StartNew(() => SendInternal(body, messageId, callerName));
        }

        private void ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            // TODO : logger
        }
    }
}
