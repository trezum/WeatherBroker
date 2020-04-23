using RabbitMQ.Client;
using System;
namespace Utility
{
    public static class ChannelFactory
    {
        private static IConnection connection;

        private static IConnection _connection
        {
            get
            {
                // Use one connection for the whole program if possible, fewer is better.
                if (connection == null || !connection.IsOpen)
                {
                    ConnectionFactory factory = new ConnectionFactory();
                    factory.Uri = new Uri("amqp://guest:guest@localhost:5672/");

                    connection = factory.CreateConnection();
                }
                return connection;
            }
        }

        public static IModel CreateQueueChannel(string queueName)
        {
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName + "Queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            return channel;
        }

        public static IModel CreateFanoutChannel(string exchangeName, string[] optionalQueueNames)
        {
            if (optionalQueueNames.Length <= 1)
            {
                throw new Exception("Fanout channels must/should have multiple queues");
            }

            var actualExchangeName = exchangeName + "Exchange";

            IModel channel = _connection.CreateModel();

            channel.ExchangeDeclare(actualExchangeName, ExchangeType.Fanout);

            foreach (var qName in optionalQueueNames)
            {
                channel.QueueDeclare(qName + "Queue", false, false, false, null);
                channel.QueueBind(qName + "Queue", actualExchangeName, "");
            }

            return channel;
        }

        public static IModel CreateDirectChannel(string name, string optionalRoutingKey = null, string[] optionalQueueNames = null)
        {
            var exchangeName = name + "Exchange";
            var queueName = name + "Queue";
            var routingKey = string.IsNullOrWhiteSpace(optionalRoutingKey) ? name + "RoutingKey" : optionalRoutingKey;

            IModel channel = _connection.CreateModel();

            if (optionalQueueNames != null && optionalQueueNames.Length > 0)
            {
                foreach (var qName in optionalQueueNames)
                {
                    channel.ExchangeDeclare(qName + "Exchange", ExchangeType.Direct);
                    channel.QueueDeclare(qName + "Queue", false, false, false, null);
                    channel.QueueBind(qName + "Queue", qName + "Exchange", routingKey, null);
                }
            }
            else
            {
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                channel.QueueDeclare(queueName, false, false, false, null);
                channel.QueueBind(queueName, exchangeName, routingKey, null);
            }

            return channel;
        }

        public static void Dispose()
        {
            _connection.Dispose();
        }
    }
}