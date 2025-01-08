using System.Text.Json.Serialization;

public class WeatherComment
{
    public int Id { get; set; }
    public string CommentMessage { get; set; }
    
    public int WeatherForecastId { get; set; }

    [JsonIgnore]
    public WeatherForecast? WeatherForecast { get; set; } = null!;
}