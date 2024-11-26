In today's lesson we'll look at one-to-one relationships in Entity Framework.  A one-to-one relationship is when each entity instance from one table is associated with only one entity instance in another table. This means that both tables share a unique link between their records, often through a primary key in one table that also acts as a foreign key in the related table. This type of relationship is used when you want to model exclusive relationships, such as a WeatherForecast having a unique alert message, where each Weatherforecast has exactly one Alert, and each Alert belongs to exactly one WeatherForecast.

``` cs
public class WeatherAlert
{
    public int Id { get; set; }
    public string AlertMessage { get; set; }
    
    public int WeatherForecastId { get; set; }

    public WeatherForecast? WeatherForecast { get; set; } = null!;
}
```

``` cs
public class WeatherForecast
{
    ...

    public int TemperatureF { get; set; }

    public WeatherAlert? Alert {get; set; }

    ...
}
```

``` cs
using Microsoft.EntityFrameworkCore;

namespace MyFirstApi.Models
{
    public class WeatherForecastDbContext : DbContext
    {
        public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options): base(options) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public DbSet<WeatherAlert> WeatherAlerts { get; set; }
    }
}

```

`dotnet ef migrations add AddWeatherAlertTable`

`dotnet run`

``` json
{
    "Date": "2024-11-24", 
    "TemperatureC": 60, 
    "Summary": "123",
    "Alert": {
        "AlertMessage": "High winds expected"
    }
}
```

It fails because the Alert needs a reference to the WeatherForecast

``` cs
using System.Text.Json.Serialization;

public class WeatherAlert
{
    public int Id { get; set; }
    public string AlertMessage { get; set; }
    
    public int WeatherForecastId { get; set; }

    [JsonIgnore]
    public WeatherForecast? WeatherForecast { get; set; } = null!;
}
```

- In ASP.NET, when a model is created from JSON, the JsonIgnore attribute prevents the corresponding property from being populated during deserialization, ensuring that data from the incoming JSON does not modify that property. Deserialization is the process of converting one set of values, in this case these values are formatted as JSON, into a corresponding object or data structure.


``` cs
namespace MyFirstApi.Models
{
    using Microsoft.EntityFrameworkCore;

    public class WeatherForecastRepository : IWriteRepository<int, WeatherForecast>, IReadRepository<int, WeatherForecast>
    {
        ...

        public WeatherForecast Save(WeatherForecast weatherForecast)
        {
            if(weatherForecast.Alert != null)
            {
                var alert = weatherForecast.Alert;
                alert.WeatherForecast = weatherForecast;
                context.WeatherAlerts.Add(alert);
            }
            context.WeatherForecasts.Add(weatherForecast);

            context.SaveChanges();
            return weatherForecast;
        }
        ...
        public List<WeatherForecast> FindAll()
        {
            return context.WeatherForecasts
            .Include(f => f.Alert)
            .ToList();
        }
        ...
    }
}
```

`dotnet run`

``` json
{
    "Date": "2024-11-24", 
    "TemperatureC": 60, 
    "Summary": "123",
    "Alert": {
        "AlertMessage": "High winds expected"
    }
}
```

In the coding exercise you will create a 1 to 1 relationship.

## Main Points
- A one-to-one relationship is when each entity instance from one table is associated with only one entity instance in another table.
- Both tables share a unique link between their records, often through a primary key in one table that also acts as a foreign key in the related table.
- The JsonIgnore attribute prevents the corresponding property from being populated during deserialization.
- Deserialization is the process of converting one set of values into a corresponding object or data structure.

## Suggested Coding Exercise
- Create a one-to-one relationship between a person and a passport.

## Building toward CSTA Standards:
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects (3A-AP-17) https://www.csteachers.org/page/standards
- Create artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs (3A-AP-18) https://www.csteachers.org/page/standards
- Compare and contrast fundamental data structures and their uses (3B-AP-12) https://www.csteachers.org/page/standards
- Construct solutions to problems using student-created components, such as procedures, modules and/or objects (3B-AP-14) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards
- Modify an existing program to add additional functionality and discuss intended and unintended implications (e.g., breaking other functionality) (3B-AP-22) https://www.csteachers.org/page/standards

## Resources
- https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-one
- https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/ignore-properties
- https://en.wikipedia.org/wiki/Serialization
