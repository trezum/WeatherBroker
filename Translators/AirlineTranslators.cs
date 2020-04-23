using DTO;
using Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using Utility;

namespace Translators
{
    internal class AirlineTranslators : IRunnable
    {
        List<IDisposable> openResources = new List<IDisposable>();
        public void Dispose()
        {
            foreach (var resource in openResources)
            {
                resource?.Dispose();
            }
        }

        public void Run()
        {
            Console.WriteLine("Starting Translators");
            SetupAirlineCompaniesSasTranslator();
            SetupAirlineCompaniesKlmTranslator();
            SetupAirlineCompaniesSwaAndBaTranslator();
        }
        private void SetupAirlineCompaniesSwaAndBaTranslator()
        {
            var filterQueueName = "AirlineCompaniesXML";
            var outputQueueNames = new string[] { "AirlineCompaniesSWA", "AirlineCompaniesBA" };
            var exchangeName = "AirlineCompaniesExchange";

            var filterChannel = ChannelFactory.CreateQueueChannel(filterQueueName);
            openResources.Add(filterChannel);

            var outputChannel = ChannelFactory.CreateFanoutChannel(exchangeName, outputQueueNames);
            openResources.Add(outputChannel);

            var consumer = new EventingBasicConsumer(filterChannel);
            consumer.Received += (ch, ea) =>
            {
                var bluffWeather = JsonSerializer.Deserialize<BluffWeatherAirline>(Encoding.ASCII.GetString(ea.Body));

                // Translation to XML
                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(bluffWeather.GetType());
                    serializer.Serialize(stringwriter, bluffWeather);

                    outputChannel.BasicPublish(exchangeName + "Exchange", exchangeName + "RoutingKey", null, Encoding.ASCII.GetBytes(stringwriter.ToString()));
                    filterChannel.BasicAck(ea.DeliveryTag, false);
                    Console.WriteLine("AirlineCompanies sent SWA and BA as XML.");
                }
            };
            var consumerTag = filterChannel.BasicConsume(filterQueueName + "Queue", false, consumer);
        }
        private void SetupAirlineCompaniesKlmTranslator()
        {
            var filterQueueName = "AirlineCompaniesString";
            var outputQueueName = "AirlineCompaniesKLM";

            var filterChannel = ChannelFactory.CreateQueueChannel(filterQueueName);
            openResources.Add(filterChannel);

            var outputChannel = ChannelFactory.CreateDirectChannel(outputQueueName);
            openResources.Add(outputChannel);

            var consumer = new EventingBasicConsumer(filterChannel);
            consumer.Received += (ch, ea) =>
            {
                var bluffWeather = JsonSerializer.Deserialize<BluffWeatherAirline>(Encoding.ASCII.GetString(ea.Body));

                // Translation to String
                var weatherString = bluffWeather.Country + "," + bluffWeather.NameOfCity + "," + bluffWeather.Temperature + "," + bluffWeather.Clouds;

                outputChannel.BasicPublish(outputQueueName + "Exchange", outputQueueName + "RoutingKey", null, Encoding.ASCII.GetBytes(weatherString));

                filterChannel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine("AirlineCompanies sent KLM as String.");
            };
            var consumerTag = filterChannel.BasicConsume(filterQueueName + "Queue", false, consumer);
        }
        private void SetupAirlineCompaniesSasTranslator()
        {
            var filterQueueName = "AirlineCompaniesClass";
            var outputQueueName = "AirlineCompaniesSAS";

            var filterChannel = ChannelFactory.CreateQueueChannel(filterQueueName);
            openResources.Add(filterChannel);

            var outputChannel = ChannelFactory.CreateDirectChannel(outputQueueName);
            openResources.Add(outputChannel);

            var consumer = new EventingBasicConsumer(filterChannel);
            consumer.Received += (ch, ea) =>
            {
                // Translation to Class? Must mean JSON right?
                outputChannel.BasicPublish(outputQueueName + "Exchange", outputQueueName + "RoutingKey", null, ea.Body);

                filterChannel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine("AirlineCompanies sent SAS as Class.");
            };
            var consumerTag = filterChannel.BasicConsume(filterQueueName + "Queue", false, consumer);
        }
    }
}