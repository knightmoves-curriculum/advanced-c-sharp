In today's lesson we'll look at global error handling.  Earlier we learned how to catch exceptions and return custom responses.  While this can be a good practice, we should always set up global error handling to catch all errors.  Without global error handling raw exceptions with their stack traces would be displayed to the client.  Allowing a stack trace to be shown to clients exposes sensitive information about the application's internal structure and potential vulnerabilities, which can be exploited by attackers. It is best practice to display a generic error message to users while logging detailed information internally for security and debugging purposes.

``` cs
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IReadRepository<int, WeatherForecast> repository;
        private IDateQueryable<WeatherForecast> forecastByDateRepository;

        public WeatherForecastController(IReadRepository<int, WeatherForecast> repository, IDateQueryable<WeatherForecast> forecastByDateRepository)
        {
            this.repository = repository;
            this.forecastByDateRepository = forecastByDateRepository;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] DateOnly? date)
        {
            if(date != null)
            {
                return Ok(forecastByDateRepository.FindByDate((DateOnly)date));
            } else {
                return Ok(repository.FindAll()); 
            }
        }

        [HttpGet("{id}")]
        public IActionResult FindById(int id)
        {
            if (id > repository.FindAll().Count)
            {
                return NotFound();
            }
            var weatherForecast = repository.FindById(id);
            return Ok(weatherForecast);
        }

        [HttpGet("boom")]
        public IActionResult Boom()
        {
            throw new InvalidOperationException("boom!");
        }
    }
}
```

``` cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        // Add logging here

        var response = new
        {
            message = "An unexpected error occurred.",
            error = context.Exception.Message
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = 500
        };
    }
}
```

``` cs
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;
using MyFirstApi.Services;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<WeatherForecastRepository>();
builder.Services.AddScoped<IReadRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddScoped<IWriteRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddScoped<IDateQueryable<WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());

builder.Services.AddScoped<CityRepository>();
builder.Services.AddScoped<IReadRepository<int, City>>(provider => provider.GetRequiredService<CityRepository>());
builder.Services.AddScoped<IWriteRepository<int, City>>(provider => provider.GetRequiredService<CityRepository>());

builder.Services.AddScoped<CityWeatherForecastRepository>();
builder.Services.AddScoped<IReadRepository<int, CityWeatherForecast>>(provider => provider.GetRequiredService<CityWeatherForecastRepository>());
builder.Services.AddScoped<IWriteRepository<int, CityWeatherForecast>>(provider => provider.GetRequiredService<CityWeatherForecastRepository>());

builder.Services.AddTransient<CurrentWeatherForecastService>();
builder.Services.AddHttpClient<CurrentWeatherForecastService>();

builder.Services.AddTransient<CityForecastService>();

builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
    options.UseSqlite("Data Source=weatherForecast.db"));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddAutoMapper(typeof(WeatherForecastProfile));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeatherForecastDbContext>();
    db.Database.Migrate();
}

app.MapControllers();

app.Run();

```

In the coding exercise you will add global error handling.

## Main Points
- we should always set up global error handling to catch all errors.
- Without global error handling raw exceptions with their stack traces would be displayed to the client.
- Allowing a stack trace to be shown to clients exposes sensitive information about the application's internal structure and potential vulnerabilities, which can be exploited by attackers.

## Suggested Coding Exercise
- Set up global error handling

## Building toward CSTA Standards:
- Develop guidelines that convey systematic troubleshooting strategies that others can use to identify and fix errors (3A-CS-03) https://www.csteachers.org/page/standards
- Recommend security measures to address various scenarios based on factors such as efficiency, feasibility, and ethical impacts (3A-NI-06) https://www.csteachers.org/page/standards
- Document design decisions using text, graphics, presentations, and/or demonstrations in the development of complex programs (3A-AP-23) https://www.csteachers.org/page/standards
- Explain security issues that might lead to compromised computer programs (3B-AP-18) https://www.csteachers.org/page/standards

## Resources
- https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-9.0#exception-filters
