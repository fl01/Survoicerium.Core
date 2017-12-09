﻿using System;
using System.Runtime.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Survoicerium.Messaging.Serialization;

namespace Survoicerium.Messaging.RabbitMq
{
    public abstract class RabbitMqQueueSubscriber
    {
        private readonly IMessageSerializer _serializer;
        private readonly string _host;
        private readonly string _password;
        private readonly string _user;
        private string _queueName;

        private IConnection _connection;
        private IModel _channel;

        public RabbitMqQueueSubscriber(string host, string user, string password, IMessageSerializer serializer, string queueName = "")
        {
            _host = host;
            _password = password;
            _user = user;
            _serializer = serializer;
            _queueName = queueName;
        }

        public void Start()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _host,
                Password = _password,
                UserName = _user,

                RequestedHeartbeat = 5,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            _connection = connectionFactory.CreateConnection();
            _connection.ConnectionShutdown += ConnectionShutdown;
            _channel = _connection.CreateModel();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += OnMessageReceived;
            consumer.Shutdown += HandleShutdown;
            //if we have the queue name to user
            if (!string.IsNullOrEmpty(_queueName))
            {
                _channel.BasicConsume(_queueName, false, consumer);
            }
            else
            {
                //generic queue with autogenerated name
                _queueName = _channel.QueueDeclare().QueueName;
                //subscribe to events topic
                _channel.QueueBind(_queueName, RabbitMqConsts.EventsExchangeName, "*");
                _channel.BasicConsume(_queueName, true, consumer);
            }
        }

        protected abstract bool ProcessMessage(string messageId, byte[] body);

        private void HandleShutdown(object sender, ShutdownEventArgs e)
        {
            // TODO : logs
        }

        private void ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            // TODO : logs
        }

        private void OnMessageReceived(object sender, BasicDeliverEventArgs deliveryArgs)
        {
            var channel = (EventingBasicConsumer)sender;
            try
            {
                string messageId = deliveryArgs.BasicProperties?.MessageId;
                if (string.IsNullOrEmpty(messageId))
                {
                    // TODO : logger
                    Console.WriteLine($"Invalid messageId '{messageId}'");
                    return;
                }

                // TODO: this is really bad approach, should be reworked later to use multiple queues
                if (ProcessMessage(messageId, deliveryArgs.Body))
                {
                    channel.Model.BasicAck(deliveryArgs.DeliveryTag, false);
                }
                else
                {
                    channel.Model.BasicNack(deliveryArgs.DeliveryTag, false, true);
                }
            }
            catch (SerializationException ex)
            {
                // TODO : logger
                Console.WriteLine($"Failed to serialize message. Error {ex.ToString()}");
                channel.Model.BasicNack(deliveryArgs.DeliveryTag, false, false);
            }
            catch (Exception ex)
            {
                // TODO : logger
                Console.WriteLine($"Unknown error {ex.ToString()}");
            }
        }
    }
}
