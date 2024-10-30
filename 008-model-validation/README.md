In today's lesson we'll make sure that we create valid weather forecasts.  For example weather temperatures have a reasonable range of values.  To date the hottest temperature recorded on earth, as of 2024, was 134.1 degrees Fahrenheit or 56.7 in Celcius.  The coldest temperature on record, to date, was negative 128.6 degrees in Fahrenheit or -89.2 in Celsius.  So it is reasonable to say that when we create new weather forecasts we can limit the temperature in celcius to a range of numbers between 60 and -90. ASP.Net Core provides validation attributes that can be added to our weather forecast model.

``` cs
using System;
using System.ComponentModel.DataAnnotations;

public class WeatherForecast
{
    [Required]
    public DateOnly Date { get; init; }

    [Required]
    [Range(-50, 60, ErrorMessage = "Temperature must be between -50 and 60 degrees Celsius.")]
    public int TemperatureC { get; init; }

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Summary must be between 3 and 20 characters.")]
    public string? Summary { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public WeatherForecast(DateOnly date, int temperatureC, string? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }
}
```
- Required: Validates that the field isn't null.
- Range: Validates that the property value falls within a specified number range.
- StringLength: Validates that a string property value doesn't exceed a specified length limit.

In the coding exercise, you will use model validation.

## Main Points
- ASP.Net Core provides validation attributes that can be added to our weather forecast model.
- Required: Validates that the field isn't null.
- Range: Validates that the property value falls within a specified number range.
- StringLength: Validates that a string property value doesn't exceed a specified length limit.

## Suggested Coding Exercise
- Have them add validation to their person model

## Building toward CSTA Standards:
- Create computational models that represent the relationships among different elements of data collected from a phenomenon or process. (3A-DA-12) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards
## Resources
- https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-8.0#validation-attributes
