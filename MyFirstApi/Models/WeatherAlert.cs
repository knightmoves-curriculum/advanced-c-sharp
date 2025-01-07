using System.Text.Json.Serialization;

public class WeatherAlert
{
    public int Id { get; set; }
    public string AlertMessage { get; set; }
    
    public int WeatherForecastId { get; set; }

    [JsonIgnore]
    public WeatherForecast? WeatherForecast { get; set; } = null!;
}