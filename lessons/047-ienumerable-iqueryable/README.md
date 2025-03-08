In today's lesson we'll look at the difference between IEnumerable<T> and IQueryable<T>.  IEnumerable<T> is an interface in C# that allows iteration over a collection of objects in memory, supporting deferred execution. In contrast, IQueryable<T> extends IEnumerable<T> and is designed for querying data from external sources like databases, enabling LINQ-to-SQL or LINQ-to-Entities to translate queries into SQL for optimized execution. The key difference is that IEnumerable<T> executes queries in memory, retrieving all data first, while IQueryable<T> constructs queries that are executed at the database level, improving performance for large datasets. Therefore, use IQueryable<T> when querying databases to leverage filtering and pagination at the source, and use IEnumerable<T> when working with in-memory collections.

``` cs

using Microsoft.Extensions.Caching.Memory;
using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class CurrentWeatherForecastService
    {
        private HttpClient httpClient;
        private IWriteRepository<int, WeatherForecast> repository;
        private readonly IMemoryCache cache;
        private readonly ILogger<CurrentWeatherForecastService> logger;
        private const string CacheKey = "CurrentWeatherForecast";

        public CurrentWeatherForecastService(HttpClient httpClient, 
                                                IWriteRepository<int, WeatherForecast> repository, 
                                                IMemoryCache cache, 
                                                ILogger<CurrentWeatherForecastService> logger)
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppName/1.0");
            this.httpClient = httpClient;
            this.repository = repository;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<WeatherForecast> Report()
        {
            if (cache.TryGetValue(CacheKey, out WeatherForecast cachedForecast))
            {
                logger.LogInformation("Returning weather forecast from cache.");
                return cachedForecast!;
            }
            
            logger.LogInformation("Calling https://api.weather.gov to retrieve forecast");
            var response = await httpClient.GetFromJsonAsync<WeatherApiResponse>("https://api.weather.gov/gridpoints/DMX/73,49/forecast");

            IEnumerable<Period> periods = response?.Properties?.Periods;

            var weatherForecastsFromDeclarativeLinq = (from period in periods
                        where period.Name == "This Afternoon"
                        select new WeatherForecast(period.StartTime, period.Temperature, period.ShortForecast))
                        .ToList();
                        
            var weatherForecastsFromFluentLinq = periods
                        .Where(p => p.Name == "This Afternoon")
                        .AsParallel()
                        .Select(p => new WeatherForecast(p.StartTime, p.Temperature, p.ShortForecast))
                        .ToList();

            var weatherForecasts = weatherForecastsFromDeclarativeLinq;

            repository.Save(weatherForecasts[0]);

            cache.Set(CacheKey, weatherForecasts[0], TimeSpan.FromSeconds(10));

            return weatherForecasts[0];
        }
    }
}

public class WeatherApiResponse
{
    public required Properties Properties { get; set; }
}

public class Properties
{
    public required List<Period> Periods { get; set; }
}

public class Period
{
    public required string Name { get; set; }
    public DateTime StartTime { get; set; }
    public double Temperature { get; set; }
    public required string ShortForecast { get; set; }
}
```

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

            var weatherForecasts = context.WeatherForecasts
            .AsEnumerable<WeatherForecast>()
            .Where(wf => dateEquals(wf));

            return context.WeatherForecasts
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

`dotnet run`

In the coding exercise you will use IEnumerable<T> and  IQueryable<T>.

## Main Points
- IEnumerable<T> is an interface in C# that allows iteration over a collection of objects in memory, supporting deferred execution.
- IQueryable<T> extends IEnumerable<T> and is designed for querying data from external sources like databases, enabling LINQ-to-SQL or LINQ-to-Entities to translate queries into SQL for optimized execution.
- use IQueryable<T> when querying databases to leverage filtering and pagination at the source
- use IEnumerable<T> when working with in-memory collections

## Suggested Coding Exercise
- Have students use IEnumerable<T> and  IQueryable<T>.

## Building toward CSTA Standards:
None

## Resources
- https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable?view=net-9.0
- https://learn.microsoft.com/en-us/dotnet/api/system.linq.iqueryable?view=net-9.0