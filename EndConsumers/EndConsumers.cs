using Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace EndConsumers
{
    internal class EndConsumers : IRunnable
    {
        List<IDisposable> openResources = new List<IDisposable>();
        public void Dispose()
        {
            foreach (var resource in openResources)
            {
                resource?.Dispose();
            }
        }

        void IRunnable.Run()
        {
            Console.WriteLine("Starting EndConsumers");
            var endConsumerQueNames = new string[]
{
                "AirlineCompaniesSAS",
                "AirlineCompaniesKLM",
                "AirlineCompaniesSWA",
                "AirlineCompaniesBA",
                "AirportInformationCenter",
                "AirTrafficControlCenter"
};
            foreach (var queueName in endConsumerQueNames)
            {
                var model = ChannelFactory.CreateQueueChannel(queueName);
                openResources.Add(model);

                var consumer = new EventingBasicConsumer(model);
                consumer.Received += (ch, ea) =>
                {
                    Console.WriteLine(queueName + "Recived: " + Encoding.ASCII.GetString(ea.Body));
                    model.BasicAck(ea.DeliveryTag, false);
                };
                var consumerTag = model.BasicConsume(queueName + "Queue", false, consumer);
            }
            Console.ReadKey();
        }
    }
}