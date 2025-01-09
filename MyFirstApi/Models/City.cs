public class City
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<CityWeatherForecast> CityWeatherForecasts { get; set; }
}
