using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Survoicerium.Messaging.Events;
using Survoicerium.Messaging.Serialization;

namespace Survoicerium.Messaging.RabbitMq
{
    public class RabbitMqBus : IMessageBus
    {
        private IModel _channel;
        private readonly IMessageSerializer _serializer;

        public RabbitMqBus(string rabbitHostName, string rabbitLogin, string rabbitPassword, IMessageSerializer serializer)
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
        }

        public Task PublishAsync<TMessage>(TMessage body)
            where TMessage : Message
        {
            return PublishAsync(body, RabbitMqConsts.ExchangeName, "*");
        }

        private Task PublishAsync<TMessage>(TMessage body, string exchange, string routingKey)
            where TMessage : Message
        {
            try
            {
                byte[] data = _serializer.Serialize(body);
                var properties = new RabbitMQ.Client.Framing.BasicProperties()
                {
                    MessageId = typeof(TMessage).Name,
                    CorrelationId = body.CorrelationId
                };

                _channel.BasicPublish(exchange, routingKey, properties, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Task.CompletedTask;
        }

        private void ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            // TODO : logger
        }
    }
}
