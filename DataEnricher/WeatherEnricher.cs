using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using DTO;
using System.Linq;
using System;
using Utility;
using RabbitMQ.Client;
using System.Text;
using Interfaces;

namespace DataEnricher
{
    public class WeatherEnricher : IRunnableAsync
    {
        private readonly HttpClient _client = null;
        private readonly IModel _fanoutChannel = null;
        private const string _channelName = "BluffWeatherCanon";

        public void Dispose()
        {
            _client?.Dispose();
            _fanoutChannel?.Dispose();
        }

        public WeatherEnricher()
        {
            var fanoutQueueNames = new string[]
            {
                "AirlineCompaniesFilter",
                "AirportInformationCenterFilter",
                "AirTrafficControlCenterFilter"
            };
            _client = new HttpClient();
            _fanoutChannel = ChannelFactory.CreateFanoutChannel("BluffWeatherCanon", fanoutQueueNames);
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Starting WeatherEnricher");
            BluffWeather lastUpdate = null;

            while (true)
            {
                // Calling open weather map
                var openWeatherModel = await GetCurrentWeather();

                // Translating to local model and filtering excess data.
                var localModel = TranslateToLocalModel(openWeatherModel);

                // Check if data has been changed since last update
                // Could be expanded to affect wait time.
                if (lastUpdate == null || !lastUpdate.Equals(localModel))
                {
                    //Send update to system
                    ProduceUpdate(localModel);
                    lastUpdate = localModel;
                    Console.WriteLine("Updates produced.");
                }
                else
                {
                    Console.WriteLine("Data was the same as last update. No Updates produced.");
                }
                //ProduceUpdate(localModel);
                Thread.Sleep(2000);
            }
        }

        private void ProduceUpdate(BluffWeather bluffWeather)
        {
            _fanoutChannel.BasicPublish(_channelName + "Exchange", "", null, Encoding.ASCII.GetBytes(JsonSerializer.Serialize(bluffWeather)));
        }

        private BluffWeather TranslateToLocalModel(OpenWeatherModel source)
        {
            return new BluffWeather()
            {
                Clouds = source.clouds.all,
                Country = source.sys.country,
                Humidity = source.main.humidity,
                Lat = source.coord.lat,
                Lon = source.coord.lon,
                NameOfCity = source.name,
                Pressure = source.main.pressure,
                Temperature = source.main.temp,
                WindDeg = source.wind.deg,
                WindSpeed = source.wind.speed,
                Visibility = source.weather.FirstOrDefault()?.main,
                Sunrise = source.sys.sunrise,
                Sunset = source.sys.sunset,
            };
        }

        private async Task<OpenWeatherModel> GetCurrentWeather()
        {
            var apiKey = "";
            var uri = "https://api.openweathermap.org/data/2.5/weather?q=aarhus&units=metric&appid=" + apiKey;
            var streamTask = _client.GetStreamAsync(uri);
            return await JsonSerializer.DeserializeAsync<OpenWeatherModel>(await streamTask);
        }
    }
}
