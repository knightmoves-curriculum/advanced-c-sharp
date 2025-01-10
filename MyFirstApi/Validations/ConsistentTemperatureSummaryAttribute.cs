using System.ComponentModel.DataAnnotations;

public class ConsistentTemperatureSummaryAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not WeatherForecastDto forecast)
        {
            return new ValidationResult("Invalid weather forecast data.");
        }
        
        if ((forecast.Summary == "Hot" && forecast.TemperatureF < 90) ||
            (forecast.Summary == "Cold" && forecast.TemperatureF > 30))
        {
            return new ValidationResult("The temperature does not match the summary description.");
        }

        return ValidationResult.Success;
    }
}