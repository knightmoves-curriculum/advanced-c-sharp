In today's lesson we'll look at Null Coalescing.  The null-coalescing operator (??) in C# provides a default value when the left-hand operand is null, returning the right-hand operand instead.

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

            var periods = response?.Properties?.Periods ?? new List<Period>();

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

`dotnet run`

In the coding exercise you will use Null Coalescing.

## Main Points
- The null-coalescing operator (??) in C# provides a default value when the left-hand operand is null, returning the right-hand operand instead.

## Suggested Coding Exercise
- Have students use Null Coalescing.

## Building toward CSTA Standards:
None

## Resources
- https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator