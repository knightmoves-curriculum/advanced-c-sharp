In today's lesson we'll look at how one API can call another API.  In order to achieve this we will need to use the Task-based Asynchronous Pattern which is also know as TAP.  This pattern is used in .NET to handle blocking opperations through asynchronous calls by using Task objects which improve application responsiveness.  Without TAP, 3rd party API call that take a long time to respond will block our code.  Instead of blocking our code TAP allows our API to request a 3rd party API asyncronously, allowing it to respond when it is ready without blocking our code.  


``` cs
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("admin/weatherforecast")]
    public class WeatherForecastAdminController : ControllerBase
    {
        private IWriteRepository<int, WeatherForecast> repository;
        private CurrentWeatherForecastService currentWeatherForecastService;

        public WeatherForecastAdminController(IWriteRepository<int, WeatherForecast> repository, CurrentWeatherForecastService currentWeatherForecastService)
        {
            this.repository = repository;
            this.currentWeatherForecastService = currentWeatherForecastService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast weatherForecast)
        {
            repository.Save(weatherForecast);
            return Created($"/weatherforecast/{repository.Count() - 1}", weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            if (id > (repository.Count() - 1))
            {
                return NotFound();
            }
            repository.Update(id, weatherForecast);
            return Ok(weatherForecast);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id > (repository.Count() - 1))
            {
                return NotFound();
            }
            var weatherForecast = repository.RemoveById(id);
            return Ok(weatherForecast);
        }

        [HttpPost("current")]
        public async Task<IActionResult> Current(){
            WeatherForecast weatherForecast = await currentWeatherForecastService.Report();
            return Ok(weatherForecast);
        }
    }
}
```
The `async` keyword is used mark a method as asynchronous and typically returns a Task.
This asynchronous method can contain one or more `await` expressions. `await` enables the method to run a command without blocking the calling thread.
Threads are lightweight unit of execution within a computer program that allows multiple tasks to run simultaneously.
``` cs

using Microsoft.AspNetCore.Authorization.Infrastructure;
using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class CurrentWeatherForecastService
    {
        private HttpClient httpClient;
        private IWriteRepository<int, WeatherForecast> repository;

        public CurrentWeatherForecastService(HttpClient httpClient, IWriteRepository<int, WeatherForecast> repository)
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppName/1.0");
            this.httpClient = httpClient;
            this.repository = repository;
        }

        public async Task<WeatherForecast> Report()
        {
            var response = await httpClient.GetFromJsonAsync<WeatherApiResponse>("https://api.weather.gov/gridpoints/DMX/73,49/forecast");

            var periods = response?.Properties.Periods;
            var weatherForecasts = (from period in periods
                        where period.Name == "Today"
                        select new WeatherForecast(period.StartTime, period.Temperature, period.ShortForecast))
                        .ToList();
            repository.Save(weatherForecasts[0]);

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

    public WeatherForecast(DateTime date, double temperature, string? summary)
    {
        Date = DateOnly.FromDateTime(date);
        TemperatureC = (int)((temperature - 32) * 5.0 / 9.0); ;
        Summary = summary;
    }
}

```

``` cs
using MyFirstApi.Models;
using MyFirstApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<WeatherForecastRepository>();
builder.Services.AddSingleton<IReadRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddSingleton<IWriteRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());

builder.Services.AddTransient<CurrentWeatherForecastService>();
builder.Services.AddHttpClient<CurrentWeatherForecastService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

```

In the coding exercise you will apply the Task-based Asynchronous Pattern.

## Main Points
- The Task-based Asynchronous Pattern is used in .NET to handle blocking opperations through asynchronous calls by using Task objects which improving application responsiveness.
- The `async` keyword is used mark a method as asynchronous and typically returns a Task.
- The `await` keyword enables the method to run a command without blocking the calling thread.
- Threads are lightweight unit of execution within a computer program that allows multiple tasks to run simultaneously.

## Suggested Coding Exercise
- Have students look up the city and state by zipcode through https://api.zippopotam.us/us/{zipCode}.

```
{
  "post code": "50047",
  "country": "United States",
  "country abbreviation": "US",
  "places": [
    {
    "place name": "Carlisle",
    "longitude": "-93.4921",
    "state": "Iowa",
    "state abbreviation": "IA",
    "latitude": "41.4814"
    }
  ]
}
```
## Building toward CSTA Standards:
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects. (3A-AP-17) https://www.csteachers.org/page/standards
- Justify the selection of specific control structures when tradeoffs involve implementation, readability, and program performance, and explain the benefits and drawbacks of choices made. (3A-AP-15) https://www.csteachers.org/page/standards
- Construct solutions to problems using student-created components, such as procedures, modules and/or objects. (3B-AP-14) https://www.csteachers.org/page/standards
- Compare levels of abstraction and interactions between application software, system software, and hardware layers. (3A-CS-02) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs. (3B-AP-16) https://www.csteachers.org/page/standards
- Use and adapt classic algorithms to solve computational problems. (3B-AP-10) https://www.csteachers.org/page/standards
- Evaluate algorithms in terms of their efficiency, correctness, and clarity. (3B-AP-11) https://www.csteachers.org/page/standards

## Resources
- https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
- https://en.wikipedia.org/wiki/Thread_(computing)
- https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap
