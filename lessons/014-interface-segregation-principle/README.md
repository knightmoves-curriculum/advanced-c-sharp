In today's lesson we'll take a look at the Interface Segregation Principle.  The Interface Segregation Principle states that an interface should only include methods that are relevant to the client using it, meaning no client should be forced to depend on methods it doesn't use. Instead of one large, all-encompassing interface, it’s better to split it into smaller, specific ones tailored to different clients. This makes the code more flexible, easier to maintain, and reduces the chance of unnecessary dependencies.

For the weather forecast api we can split it into an api for general users and an api that is only for administrators.  
In the api available to the general public is read only while the administrator's api has write priveledges.

``` cs
namespace MyFirstApi.Models
{
    public interface IWriteRepository<TId, T>
    {
        T Save(T entity);
        T Update(TId id, T entity);
        T RemoveById(TId id);
        int Count();
    }
}
```

``` cs
namespace MyFirstApi.Models
{
    public interface IReadRepository<TId, T>
    {
        List<T> FindAll();
        T FindById(TId id);
    }
}
```

``` cs
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("admin/weatherforecast")]
    public class WeatherForecastAdminController : ControllerBase
    {
        private IWriteRepository<int, WeatherForecast> repository;

        public WeatherForecastAdminController(IWriteRepository<int, WeatherForecast> repository)
        {
            this.repository = repository;
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
    }
}
```

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

        public WeatherForecastController(IReadRepository<int, WeatherForecast> repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(repository.FindAll());
        }

        [HttpGet("{id}")]
        public IActionResult FindById(int id)
        {
            if (id > (repository.FindAll().Count - 1))
            {
                return NotFound();
            }
            var weatherForecast = repository.FindById(id);
            return Ok(weatherForecast);
        }
    }
}
```

``` cs
using MyFirstApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<WeatherForecastRepository>();
builder.Services.AddSingleton<IReadRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddSingleton<IWriteRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

```

In the coding exercise, you will apply the interface segregation principle.

## Main Points

## Suggested Coding Exercise

## Building toward CSTA Standards:

## Resources
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0#overview-of-dependency-injection
