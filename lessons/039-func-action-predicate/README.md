In today's lesson we'll look at Func, Action and Predicate. A Func in C# is a delegate that represents a method that takes one or more input parameters and returns a value.

``` cs
namespace MyFirstApi.Models
{
    using System;
    using System.Reflection.Metadata.Ecma335;
    using Microsoft.EntityFrameworkCore;
    using MyFirstApi.Pagination;

    public class WeatherForecastRepository : IWriteRepository<int, WeatherForecast>, IPaginatedReadRepository<int, WeatherForecast>, IDateQueryable<WeatherForecast>
    {
        private WeatherForecastDbContext context;

        public WeatherForecastRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

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

        public WeatherForecast Update(int id, WeatherForecast weatherForecast)
        {
            weatherForecast.Id = id;
            context.WeatherForecasts.Update(weatherForecast);
            context.SaveChanges();
            return weatherForecast;
        }

        public List<WeatherForecast> FindAll()
        {
            return context.WeatherForecasts
            .Include(f => f.Alert)
            .Include(f => f.Comments)
            .Include(f => f.CityWeatherForecasts)
            .ToList();
        }

        public WeatherForecast FindById(int id)
        {
            return context.WeatherForecasts.Find(id);
        }

        public WeatherForecast RemoveById(int id)
        {
            var weatherForecast = context.WeatherForecasts.Find(id);
            context.WeatherForecasts.Remove(weatherForecast);
            context.SaveChanges();
            return weatherForecast;
        }

        public int Count()
        {
            return context.WeatherForecasts.Count();
        }

        public List<WeatherForecast> FindByDate(DateOnly date)
        {
            Func<string, object> writeToConsole = message => {
                    Console.WriteLine(message); 
                    return null;
                };

            writeToConsole("finding by date: " + date);

            Func<WeatherForecast, bool> dateEquals = wf => { 
                    return wf.Date == date;
                };

            return context.WeatherForecasts
            .Where(wf => dateEquals(wf))
            .Include(f => f.Alert)
            .Include(f => f.Comments)
            .Include(f => f.CityWeatherForecasts)
            .ToList();
        }

        public PaginatedResult<WeatherForecast> FindPaginated(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var totalCount = context.WeatherForecasts.Count();
            var items = context.WeatherForecasts
                .Include(f => f.Alert)
                .Include(f => f.Comments)
                .Include(f => f.CityWeatherForecasts)
                .OrderBy(f => f.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<WeatherForecast>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = pageNumber * pageSize < totalCount
            };
        }

        public PaginatedResult<WeatherForecast> FindPaginatedByDate(DateOnly date, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var totalCount = context.WeatherForecasts.Count();
            var items = context.WeatherForecasts
                .Where(wf => wf.Date == date)
                .Include(f => f.Alert)
                .Include(f => f.Comments)
                .Include(f => f.CityWeatherForecasts)
                .OrderBy(f => f.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<WeatherForecast>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = pageNumber * pageSize < totalCount
            };
        }
    }
}
```
A lambda expression in C# is a concise way to define an anonymous function that can be used as a delegate which uses the syntax (parameters) => expression.

An Action in C# is a delegate that takes one or more input parameters but does not return a value (i.e., it performs an action instead of producing a result).

A Predicate in C# is a delegate that takes a single input parameter and returns a bool, typically used for filtering or condition checking.

`dotnet run`

In the coding exercise you will use Func, Action, and Predicate.

## Main Points
- A Func in C# is a delegate that represents a method that takes one or more input parameters and returns a value.
- A lambda expression in C# is a concise way to define an anonymous function that can be used as a delegate which uses the syntax (parameters) => expression.
- An Action in C# is a delegate that takes one or more input parameters but does not return a value (i.e., it performs an action instead of producing a result).
- A Predicate in C# is a delegate that takes a single input parameter and returns a bool, typically used for filtering or condition checking.

## Suggested Coding Exercise
- Have students use Func, Action, and Predicate.

## Building toward CSTA Standards:
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects (3A-AP-17) https://www.csteachers.org/page/standards
- Create artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs (3A-AP-18) https://www.csteachers.org/page/standards
- Construct solutions to problems using student-created components, such as procedures, modules and/or objects (3B-AP-14) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards
- Use lists to simplify solutions, generalizing computational problems instead of repeatedly using simple variables (3A-AP-14) https://www.csteachers.org/page/standards
- Create computational models that represent the relationships among different elements of data collected from a phenomenon or process (3A-DA-12) https://www.csteachers.org/page/standards

## Resources
- https://www.c-sharpcorner.com/article/func-delegate-in-c-sharp/
- https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions