In today's lesson we'll look at how to create custom validations.  Creating a custom validation within ASP.NET allows you to enforce specific business rules or constraints that are not covered by the built-in validation attributes, ensuring that your application's data integrity and logic requirements are met. 

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
Custom validation in can be achieved by creating a class that inherits from the ValidationAttribute class. By overriding the IsValid method, you can implement custom validation logic that check whether the data meets your criteria. This approach allows you to integrate complex validation logic into your data models, ensuring that user input is validated according to your business rules before being processed.
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
In C#, when you create a custom attribute, by convention you can drop the "Attribute" suffix when applying it because the framework automatically recognizes the class by its shorter name.

In the coding exercise you will create a custom validation.

## Main Points
- Custom validation in ASP.NET can be achieved by creating a class that inherits from the ValidationAttribute class.
- By overriding the IsValid method, you can implement custom validation logic that check whether the data meets your criteria.
- In C#, when you create a custom attribute, by convention you can drop the "Attribute" suffix when applying it because the framework automatically recognizes the class by its shorter name.

## Suggested Coding Exercise
- Add a custom validation.

## Building toward CSTA Standards:
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects (3A-AP-17) https://www.csteachers.org/page/standards
- Create artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs (3A-AP-18) https://www.csteachers.org/page/standards
- Evaluate and refine computational artifacts to make them more usable and accessible (3A-AP-21) https://www.csteachers.org/page/standards
- Systematically design and develop programs for broad audiences by incorporating feedback from users (3A-AP-19) https://www.csteachers.org/page/standards
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-9.0#custom-attributes
- https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/is
