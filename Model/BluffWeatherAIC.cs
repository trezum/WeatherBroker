namespace DTO
{
    public class BluffWeatherAIC
    {
        public BluffWeatherAIC()
        {
        }

        public BluffWeatherAIC(BluffWeather bluffWeather)
        {
            NameOfCity = bluffWeather.NameOfCity;
            Country = bluffWeather.Country;
            Temperature = bluffWeather.Temperature;
            Sunrise = bluffWeather.Sunrise;
            Sunset = bluffWeather.Sunset;
        }

        public string NameOfCity { get; set; }
        public string Country { get; set; }
        public double Temperature { get; set; }
        public int Sunrise { get; set; }
        public int Sunset { get; set; }
    }
}
