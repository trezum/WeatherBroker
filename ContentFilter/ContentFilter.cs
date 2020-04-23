using DTO;
using Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Utility;

namespace ContentFilters
{
    internal class ContentFilter : IRunnable
    {
        List<IDisposable> openResources = new List<IDisposable>();
        void IRunnable.Run()
        {
            Console.WriteLine("Starting ContentFilters");
            SetupAirlineCompaniesFilter();
            SetupAirportInformationCenterFilter();
            SetupAirTrafficControlCenterFilter();
        }
        public void Dispose()
        {
            foreach (var resource in openResources)
            {
                resource?.Dispose();
            }
        }

        private void SetupAirTrafficControlCenterFilter()
        {
            var filterQueueName = "AirTrafficControlCenterFilter";
            var outputQueueName = "AirTrafficControlCenter";

            var filterChannel = ChannelFactory.CreateQueueChannel(filterQueueName);
            openResources.Add(filterChannel);

            var outputChannel = ChannelFactory.CreateDirectChannel(outputQueueName);
            openResources.Add(outputChannel);

            var consumer = new EventingBasicConsumer(filterChannel);
            consumer.Received += (ch, ea) =>
            {
                var bluffWeather = JsonSerializer.Deserialize<BluffWeather>(Encoding.ASCII.GetString(ea.Body));

                // Translation
                var weather = new BluffWeatherATCC(bluffWeather);

                outputChannel.BasicPublish(outputQueueName + "Exchange", outputQueueName + "RoutingKey", null, Encoding.ASCII.GetBytes(JsonSerializer.Serialize(weather)));

                filterChannel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine("BluffWeatherATCC sent");
            };
            var consumerTag = filterChannel.BasicConsume(filterQueueName + "Queue", false, consumer);
        }

        private void SetupAirportInformationCenterFilter()
        {
            var filterQueueName = "AirportInformationCenterFilter";
            var outputQueueName = "AirportInformationCenter";

            var filterChannel = ChannelFactory.CreateQueueChannel(filterQueueName);
            openResources.Add(filterChannel);

            var outputChannel = ChannelFactory.CreateDirectChannel(outputQueueName);
            openResources.Add(outputChannel);

            var consumer = new EventingBasicConsumer(filterChannel);
            consumer.Received += (ch, ea) =>
            {
                var bluffWeather = JsonSerializer.Deserialize<BluffWeather>(Encoding.ASCII.GetString(ea.Body));

                // Translation
                var weather = new BluffWeatherAIC(bluffWeather);

                outputChannel.BasicPublish(outputQueueName + "Exchange", outputQueueName + "RoutingKey", null, Encoding.ASCII.GetBytes(JsonSerializer.Serialize(weather)));

                filterChannel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine("BluffWeatherAIC sent");
            };
            var consumerTag = filterChannel.BasicConsume(filterQueueName + "Queue", false, consumer);
        }

        private void SetupAirlineCompaniesFilter()
        {
            var filterQueueName = "AirlineCompaniesFilter";
            var outputQueueName = "AirlineCompanies";

            var filterChannel = ChannelFactory.CreateQueueChannel(filterQueueName);
            openResources.Add(filterChannel);

            // Fanout
            var fanoutQueueNames = new string[]
            {
                "AirlineCompaniesString",
                "AirlineCompaniesClass",
                "AirlineCompaniesXML"
            };
            var outputChannel = ChannelFactory.CreateFanoutChannel(outputQueueName, fanoutQueueNames);
            openResources.Add(outputChannel);

            var consumer = new EventingBasicConsumer(filterChannel);
            consumer.Received += (ch, ea) =>
            {
                var bluffWeather = JsonSerializer.Deserialize<BluffWeather>(Encoding.ASCII.GetString(ea.Body));

                // Translation
                var weather = new BluffWeatherAirline(bluffWeather);

                outputChannel.BasicPublish(outputQueueName + "Exchange", outputQueueName + "RoutingKey", null, Encoding.ASCII.GetBytes(JsonSerializer.Serialize(weather)));

                filterChannel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine("BluffWeatherAirline sent");
            };
            var consumerTag = filterChannel.BasicConsume(filterQueueName + "Queue", false, consumer);
        }
    }
}