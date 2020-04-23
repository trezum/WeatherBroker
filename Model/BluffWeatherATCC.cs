namespace DTO
{
    public class BluffWeatherATCC
    {
        public BluffWeatherATCC()
        {
        }

        public BluffWeatherATCC(BluffWeather bluffWeather)
        {
            NameOfCity = bluffWeather.NameOfCity;
            Lat = bluffWeather.Lat;
            Lon = bluffWeather.Lon;
            Country = bluffWeather.Country;
            Temperature = bluffWeather.Temperature;
            Pressure = bluffWeather.Pressure;
            Humidity = bluffWeather.Humidity;
            WindSpeed = bluffWeather.WindSpeed;
            WindDeg = bluffWeather.WindDeg;
            Clouds = bluffWeather.Clouds;
            Visibility = bluffWeather.Visibility;
        }

        public string NameOfCity { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Country { get; set; }
        public double Temperature { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public int WindDeg { get; set; }
        public int Clouds { get; set; }
        public string Visibility { get; set; }
    }
}
