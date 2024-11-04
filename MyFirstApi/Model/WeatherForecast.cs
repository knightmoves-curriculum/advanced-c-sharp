using System.ComponentModel.DataAnnotations;

public class WeatherForecast
{
    [Required]
    public DateOnly Date {get;}
    [Required]
    [Range(-90, 60)]
    public int TemperatureC {get;}
    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Please provide a valid summary.")]
    public string? Summary {get;}
    public int TemperatureF {get;}

    public WeatherForecast(DateTime date, double temperature, string? summary)
    {
        Date = DateOnly.FromDateTime(date);
        TemperatureF = (int) temperature;
        TemperatureC = (int)((temperature - 32) * 5.0 / 9.0); ;
        Summary = summary;
    }
}
