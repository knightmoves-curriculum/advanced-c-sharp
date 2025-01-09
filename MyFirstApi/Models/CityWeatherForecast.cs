using System.Text.Json.Serialization;

public class CityWeatherForecast
{
    public int Id { get; set; }

    public int CityId { get; set; }

    [JsonIgnore]
    public City City { get; set; }

    public int WeatherForecastId { get; set; }

    [JsonIgnore]
    public WeatherForecast WeatherForecast { get; set; }
}