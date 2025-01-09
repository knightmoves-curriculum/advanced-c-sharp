In today's lesson we'll look at how to create custom validations.  Custom validation in ASP.NET can be achieved by creating a class that inherits from the ValidationAttribute class. By overriding the IsValid method, you can implement custom validation logic that check whether the data meets your criteria. This approach allows you to integrate complex validation logic into your data models, ensuring that user input is validated according to your business rules before being processed.

``` cs
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
```

``` cs
using System.ComponentModel.DataAnnotations;


[ConsistentTemperatureSummary]
public class WeatherForecastDto
{
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Summary must be between 3 and 20 characters.")]
    public string? Summary { get; set; }

    public int TemperatureF { get; set; }

    public String? Alert { get; set; }
    public ICollection<String>? Comments { get; set; }
    public ICollection<int>? CityIds { get; set; }
}
```

In the coding exercise ...

## Main Points
- 

## Suggested Coding Exercise
- 

## Building toward CSTA Standards:
- 

## Resources
- https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-9.0#custom-attributes
