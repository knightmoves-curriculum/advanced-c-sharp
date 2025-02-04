In today's lesson we'll look at Nullable Value Types.  By default, value types (e.g., int, bool, double) cannot be null in C#. However, by using a Nullable Value Type you can explicitly allow a value type to be null by using the nullable value type syntax (?). 

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

Under the hood, int?, bool?, and other nullable types are shorthand for Nullable<T>, which is a generic struct (struct Nullable<T>) in .NET.

`dotnet run`

In the coding exercise you will use a Nullable Value Type.

## Main Points
- By default, value types (e.g., int, bool, double) cannot be null in C#.
- By using a Nullable Value Type you can explicitly allow a value type to be null by using the nullable value type syntax (?). 
- Under the hood, int?, bool?, and other nullable types are shorthand for Nullable<T>, which is a generic struct (struct Nullable<T>) in .NET.

## Suggested Coding Exercise
- Have students use a Nullable Value Type.

## Building toward CSTA Standards:
- Compare and contrast fundamental data structures and their uses (3B-AP-12) https://www.csteachers.org/page/standards


## Resources
- https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types