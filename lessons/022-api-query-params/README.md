In today's lesson we'll look at query parameters.  A query parameter is a key-value pair appended to the URL of an API request, used to pass additional information to the server. It allows clients to specify criteria such as filters, sorting, or pagination, enabling more dynamic and customized responses from the server.

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
    }
}
```

[FromQuery] is an attribute in ASP.NET Core that binds a query parameter from the URL to a method parameter in a controller action.

``` cs
using System.Collections.Generic;

namespace MyFirstApi.Models
{
    public interface IDateQueryable<T>
    {
        List<T> FindByDate(DateOnly date);
    }
}
```

``` cs
namespace MyFirstApi.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public class WeatherForecastRepository : IWriteRepository<int, WeatherForecast>, IReadRepository<int, WeatherForecast>, IDateQueryable<WeatherForecast>
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
            return context.WeatherForecasts
            .Where(wf => wf.Date == date)
            .Include(f => f.Alert)
            .Include(f => f.Comments)
            .Include(f => f.CityWeatherForecasts)
            .ToList();
        }
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

builder.Services.AddControllers();

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

```
GET http://localhost:5227/weatherforecast?date=2024-12-24
```

In the coding exercise you will use a query parameter.

## Main Points
- A query parameter is a key-value pair appended to the URL of an API request, used to pass additional information to the server.
- [FromQuery] is an attribute in ASP.NET Core that binds a query parameter from the URL to a method parameter in a controller action.

## Suggested Coding Exercise
- Filter people by last name.

## Building toward CSTA Standards:
- Compare levels of abstraction and interactions between application software, system software, and hardware layers (3A-CS-02) https://www.csteachers.org/page/standards
- Create artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs (3A-AP-18) https://www.csteachers.org/page/standards
- Use lists to simplify solutions, generalizing computational problems instead of repeatedly using simple variables (3A-AP-14) https://www.csteachers.org/page/standards
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects (3A-AP-17) https://www.csteachers.org/page/standards
- Compare and contrast fundamental data structures and their uses (3B-AP-12) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Query_string
- https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-9.0
