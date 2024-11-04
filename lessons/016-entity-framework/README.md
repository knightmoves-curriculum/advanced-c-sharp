In today's lesson we'll ...

```
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

/Models/WeatherForecastDbContext.cs
``` cs
using Microsoft.EntityFrameworkCore;

namespace MyFirstApi.Models
{
    public class WeatherForecastDbContext : DbContext
    {
        public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options): base(options) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    }
}
```

Models/WeatherForecast.cs
``` cs
using System.ComponentModel.DataAnnotations;

public class WeatherForecast
{
    [Key]
    public int Id { get; set;}
    
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [Range(-50, 60, ErrorMessage = "Temperature must be between -50 and 60 degrees Celsius.")]
    public int TemperatureC { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Summary must be between 3 and 20 characters.")]
    public string? Summary { get; set; }

    public int TemperatureF { get; set; }

    public WeatherForecast(DateTime date, double temperature, string? summary)
    {
        Date = DateOnly.FromDateTime(date);
        TemperatureC = (int)((temperature - 32) * 5.0 / 9.0); ;
        Summary = summary;
    }

    public WeatherForecast()
    {

    }
}
```

Models/WeatherForecastRepository.cs
``` cs
namespace MyFirstApi.Models
{
    public class WeatherForecastRepository : IWriteRepository<int, WeatherForecast>, IReadRepository<int, WeatherForecast>
    {
        private WeatherForecastDbContext context;

        public WeatherForecastRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

        public WeatherForecast Save(WeatherForecast weatherForecast)
        {
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
            return context.WeatherForecasts.ToList();
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
    }
}
```

Controllers/WeatherForecastAdminController.cs
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
            return Created($"/weatherforecast/{repository.Count()}", weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            if (id > repository.Count())
            {
                return NotFound();
            }
            repository.Update(id, weatherForecast);
            return Ok(weatherForecast);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id > repository.Count())
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

Controllers/WeatherForecastController.cs
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

Program.cs
``` cs
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;
using MyFirstApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<WeatherForecastRepository>();
builder.Services.AddScoped<IReadRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddScoped<IWriteRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());

builder.Services.AddTransient<CurrentWeatherForecastService>();
builder.Services.AddHttpClient<CurrentWeatherForecastService>();

builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
    options.UseSqlite("Data Source=weatherForecast.db"));

builder.Services.AddControllers();

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
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

In the coding exercise ...

## Main Points
- 

## Suggested Coding Exercise
- 

## Building toward CSTA Standards:
- 

## Resources
- 
