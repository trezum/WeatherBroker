namespace DTO
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class BluffWeather
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
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
        public int Sunrise { get; set; }
        public int Sunset { get; set; }

        public override bool Equals(object obj)
        {
            return obj is BluffWeather weather &&
                   NameOfCity == weather.NameOfCity &&
                   Lat == weather.Lat &&
                   Lon == weather.Lon &&
                   Country == weather.Country &&
                   Temperature == weather.Temperature &&
                   Pressure == weather.Pressure &&
                   Humidity == weather.Humidity &&
                   WindSpeed == weather.WindSpeed &&
                   WindDeg == weather.WindDeg &&
                   Clouds == weather.Clouds &&
                   Visibility == weather.Visibility &&
                   Sunrise == weather.Sunrise &&
                   Sunset == weather.Sunset;
        }
    }
}
