using System.ComponentModel.DataAnnotations;

public class WeatherForecast
{
    [Key]
    public int Id { get; set;}
    
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [Range(-50, 60, ErrorMessage = "Temperature must be between -50 and 60 degrees Celsius.")]
    public int TemperatureC { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Summary must be between 3 and 20 characters.")]
    public string? Summary { get; set; }

    public int TemperatureF { get; set; }

    public WeatherForecast(DateTime date, double temperature, string? summary)
    {
        Date = DateOnly.FromDateTime(date);
        TemperatureC = (int)((temperature - 32) * 5.0 / 9.0); ;
        Summary = summary;
    }

    public WeatherForecast()
    {

    }
}