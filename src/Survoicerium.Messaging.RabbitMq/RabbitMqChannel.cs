using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Survoicerium.Messaging.Messages;
using Survoicerium.Messaging.Serialization;

namespace Survoicerium.Messaging.RabbitMq
{
    public class RabbitMqChannel : IMessageChannel
    {
        private List<MessageSubscription> _subs = new List<MessageSubscription>();
        private readonly IMessageSerializer _serializer;
        private readonly string _host;
        private readonly string _password;
        private readonly string _user;
        private string _queueName;

        private IConnection _connection;
        private IModel _channel;
        private bool _isInitialized = false;

        public RabbitMqChannel(string host, string user, string password, IMessageSerializer serializer, string queueName)
        {
            _host = host;
            _password = password;
            _user = user;
            _serializer = serializer;
            _queueName = queueName;
        }

        public void On<TMessage>(Action<object> handler)
            where TMessage : Message
        {
            var sub = new MessageSubscription()
            {
                Type = typeof(TMessage),
                MessageId = typeof(TMessage).Name,
                Handler = handler
            };

            _subs.Add(sub);
        }

        public void Start()
        {
            if (_isInitialized)
            {
                return;
            }

            var connectionFactory = new ConnectionFactory
            {
                HostName = _host,
                Password = _password,
                UserName = _user,

                RequestedHeartbeat = 5,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            _isInitialized = TryInitialize(connectionFactory);
        }

        private void HandleShutdown(object sender, ShutdownEventArgs e)
        {
            // TODO : logs
        }

        private void ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            // TODO : logs
        }

        private bool TryInitialize(ConnectionFactory factory)
        {
            int maxAttempts = 5;
            int currentAttempt = 1;
            while (currentAttempt++ <= maxAttempts)
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    continue;
                }

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += OnMessageReceived;
                consumer.Shutdown += HandleShutdown;
                _channel.BasicConsume(_queueName, false, consumer);
                _connection.ConnectionShutdown += ConnectionShutdown;
                return true;
            }

            return false;
        }

        private void OnMessageReceived(object sender, BasicDeliverEventArgs deliveryArgs)
        {
            var channel = (EventingBasicConsumer)sender;
            string messageId = deliveryArgs.BasicProperties?.MessageId;
            if (string.IsNullOrEmpty(messageId))
            {
                // TODO : logger
                Console.WriteLine($"Invalid messageId '{messageId}'");
                return;
            }

            var handlers = _subs.Where(s => string.Equals(s.MessageId, messageId, StringComparison.OrdinalIgnoreCase));
            if (!handlers.Any())
            {
                // TODO : logger
                Console.WriteLine($"Event '{messageId}' is unhandled.");
            }

            foreach (var sub in handlers)
            {
                try
                {
                    var args = _serializer.Deserialize(deliveryArgs.Body, sub.Type);
                    sub.Handler(args);
                    channel.Model.BasicAck(deliveryArgs.DeliveryTag, false);
                }
                catch (JsonException ex)
                {
                    // TODO : logger
                    Console.WriteLine($"Failed to serialize body of event '{messageId}'. Error {ex}");
                    channel.Model.BasicNack(deliveryArgs.DeliveryTag, false, false);
                }
                catch (Exception ex)
                {
                    // TODO : logger
                    Console.WriteLine($"Failed to handle '{messageId}'. Error {ex}");
                    channel.Model.BasicNack(deliveryArgs.DeliveryTag, false, true);
                }
            }
        }
    }
}
