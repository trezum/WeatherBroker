namespace DTO
{
    public class BluffWeatherAirline
    {
        public BluffWeatherAirline()
        {
        }

        public BluffWeatherAirline(BluffWeather bluffWeather)
        {
            NameOfCity = bluffWeather.NameOfCity;
            Country = bluffWeather.Country;
            Temperature = bluffWeather.Temperature;
            Clouds = bluffWeather.Clouds;
        }

        public string NameOfCity { get; set; }
        public string Country { get; set; }
        public double Temperature { get; set; }
        public int Clouds { get; set; }
    }
}
