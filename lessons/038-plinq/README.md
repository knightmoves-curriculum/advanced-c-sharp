In today's lesson we'll look at PLinq. PLINQ (Parallel LINQ) is an extension of LINQ that enables parallel execution of queries to improve performance on multi-core processors. It automatically partitions data and distributes computations across multiple threads. A thread is the smallest unit of execution within a process, capable of running independently while sharing the same memory space as other threads in the process. By leveraging PLINQ, CPU-bound operations on large datasets can be processed in parallel, reducing execution time compared to sequential LINQ. However, PLINQ is most effective for computationally expensive tasks and should be used carefully to avoid unnecessary overhead.

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

            var periods = response?.Properties.Periods;

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

Declarative LINQ (query syntax) expresses operations using a SQL-like format that describes what to retrieve rather than how to retrieve it.

Fluent LINQ (method syntax) uses method chaining with lambda expressions to express how data should be queried and transformed in a functional, pipeline-style manner.

`dotnet run`

In the coding exercise you will use PLinq.

## Main Points
- PLINQ (Parallel LINQ) is an extension of LINQ that enables parallel execution of queries to improve performance on multi-core processors. 
- A thread is the smallest unit of execution within a process, capable of running independently while sharing the same memory space as other threads in the process.
- Declarative LINQ (query syntax) expresses operations using a SQL-like format that describes what to retrieve rather than how to retrieve it.
- Fluent LINQ (method syntax) uses method chaining with lambda expressions to express how data should be queried and transformed in a functional, pipeline-style manner.

## Suggested Coding Exercise
- Have students add caching to their API.

## Building toward CSTA Standards:
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects (3A-AP-17) https://www.csteachers.org/page/standards
- Create prototypes that use algorithms to solve computational problems by leveraging prior student knowledge and personal interests (3A-AP-13) https://www.csteachers.org/page/standards
- Create artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs (3A-AP-18) https://www.csteachers.org/page/standards
- Analyze a large-scale computational problem and identify generalizable patterns that can be applied to a solution (3B-AP-15) https://www.csteachers.org/page/standards
- Construct solutions to problems using student-created components, such as procedures, modules and/or objects (3B-AP-14) https://www.csteachers.org/page/standards


## Resources
- https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/introduction-to-plinq
- https://simple.wikipedia.org/wiki/Thread_(computer_science)
- https://simple.wikipedia.org/wiki/Parallel_computing