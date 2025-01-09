using System.ComponentModel.DataAnnotations;

public class WeatherForecast
{
    [Key]
    public int Id { get; set;}

    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    public int TemperatureF { get; set; }

    public WeatherAlert? Alert { get; set; }
    public ICollection<WeatherComment> Comments { get; set; }
    public ICollection<CityWeatherForecast> CityWeatherForecasts { get; set; }

    public WeatherForecast(DateTime date, double temperature, string? summary) : this( DateOnly.FromDateTime(date), (int) temperature, summary)
    {

    }

    public WeatherForecast(DateOnly date, int temperature, string? summary)
    {
        Date = date;
        TemperatureC = (int)((temperature - 32) * 5.0 / 9.0); ;
        Summary = summary;
    }

    public WeatherForecast()
    {

    }
}