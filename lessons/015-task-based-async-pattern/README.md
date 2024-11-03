In today's lesson we'll ...


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

In the coding exercise ...

## Main Points
- 

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
- 

## Resources
- 
