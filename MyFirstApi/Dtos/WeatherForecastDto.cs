using System.ComponentModel.DataAnnotations;

public class WeatherForecastDto
{
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Summary must be between 3 and 20 characters.")]
    public string? Summary { get; set; }

    [Required]
    [Range(-130, 150, ErrorMessage = "Temperature must be between -130 and 150 degrees Fahrenheit.")]
    public int TemperatureF { get; set; }

    public String? Alert { get; set; }
    public ICollection<String>? Comments { get; set; }
}