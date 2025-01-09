In today's lesson we'll look at many-to-many relationships in Entity Framework.  A many-to-many relationship is when many entity instances from one table are associated with many entity instances in another table. Often in practice this means that both tables are linked through a thrid table that holds a foreign key relationship with both of the related tables. An example of a many-to-many relationship is a City having many WeatherForecasts and a WeatherForecast relating to multiple Cities.

```cs
public class City
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<CityWeatherForecast> CityWeatherForecasts { get; set; }
}

```

```cs
using System.Text.Json.Serialization;

public class CityWeatherForecast
{
    public int Id { get; set; }

    public int CityId { get; set; }

    [JsonIgnore]
    public City City { get; set; }

    public int WeatherForecastId { get; set; }

    [JsonIgnore]
    public WeatherForecast WeatherForecast { get; set; }
}
```

```cs
using System.ComponentModel.DataAnnotations;

public class WeatherForecast
{
    [Key]
    public int Id { get; set;}

    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    public int TemperatureF { get; set; }

    public WeatherAlert? Alert { get; set; }
    public ICollection<WeatherComment> Comments { get; set; }
    public ICollection<CityWeatherForecast> CityWeatherForecasts { get; set; }

    public WeatherForecast(DateTime date, double temperature, string? summary) : this( DateOnly.FromDateTime(date), (int) temperature, summary)
    {

    }

    public WeatherForecast(DateOnly date, int temperature, string? summary)
    {
        Date = date;
        TemperatureC = (int)((temperature - 32) * 5.0 / 9.0); ;
        Summary = summary;
    }

    public WeatherForecast()
    {

    }
}

```

```cs
using Microsoft.EntityFrameworkCore;

namespace MyFirstApi.Models
{
    public class WeatherForecastDbContext : DbContext
    {
        public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options): base(options) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public DbSet<WeatherAlert> WeatherAlerts { get; set; }
        public DbSet<WeatherComment> WeatherComments { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<CityWeatherForecast> CityWeatherForecasts { get; set; }
    }
}

```

```cs
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("admin/city")]
    public class CityAdminController : ControllerBase
    {
        private IWriteRepository<int, City> repository;
        private IMapper mapper;

        public CityAdminController(IWriteRepository<int, City> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpPost]
        public IActionResult Post([FromBody] CityDto cityDto)
        {
            City city = mapper.Map<City>(cityDto);
            repository.Save(city);
            return Created($"/city/{repository.Count()}", city);
        }
    }
}
```

```cs
namespace MyFirstApi.Models
{
    using Microsoft.EntityFrameworkCore;

    public class CityRepository : IWriteRepository<int, City>, IReadRepository<int, City>
    {
        private WeatherForecastDbContext context;

        public CityRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

        public City Save(City city)
        {
            context.Cities.Add(city);
            context.SaveChanges();
            return city;
        }

        public City Update(int id, City city)
        {
            city.Id = id;
            context.Cities.Update(city);
            context.SaveChanges();
            return city;
        }

        public City RemoveById(int id)
        {
            var city = context.Cities.Find(id);
            context.Cities.Remove(city);
            context.SaveChanges();
            return city;
        }

        public List<City> FindAll()
        {
            return context.Cities
            .ToList();
        }

        public City FindById(int id)
        {
            return context.Cities.Find(id);
        }

        public int Count()
        {
            return context.Cities.Count();
        }
    }
}
```

```cs
public class CityDto
{
    public string Name { get; set; }
}
```

```cs
using AutoMapper;

public class WeatherForecastProfile : Profile
{
    public WeatherForecastProfile()
    {
        CreateMap<WeatherForecastDto, WeatherForecast>()
            .ForMember(dest => dest.TemperatureC, 
                       opt => opt.MapFrom(src => (src.TemperatureF - 32) * 5 / 9))
            .ForMember(dest => dest.Alert, 
                       opt => opt.MapFrom(src => src.Alert != null 
                                                 ? new WeatherAlert { AlertMessage = src.Alert } 
                                                 : null))
            .ForMember(dest => dest.Comments, 
                       opt => opt.MapFrom(src => src.Comments != null 
                                                 ? src.Comments.Select(c => new WeatherComment { CommentMessage = c }).ToList() 
                                                 : new List<WeatherComment>()));
        CreateMap<CityDto, City>();
    }
}
```

```cs
using AutoMapper;
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
        private CityForecastService cityForecastService;
        private IMapper mapper;

        public WeatherForecastAdminController(IWriteRepository<int, WeatherForecast> repository,
                                                CityForecastService cityForecastService, 
                                                CurrentWeatherForecastService currentWeatherForecastService,
                                                IMapper mapper)
        {
            this.repository = repository;
            this.cityForecastService = cityForecastService;
            this.currentWeatherForecastService = currentWeatherForecastService;
            this.mapper = mapper;
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecastDto weatherForecastDto)
        {
            WeatherForecast weatherForecast = mapper.Map<WeatherForecast>(weatherForecastDto);
            repository.Save(weatherForecast);
            cityForecastService.Associate(weatherForecast, weatherForecastDto.Cities);
            return Created($"/weatherforecast/{repository.Count()}", weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] WeatherForecastDto weatherForecastDto, [FromRoute] int id)
        {
            WeatherForecast weatherForecast = mapper.Map<WeatherForecast>(weatherForecastDto);
            
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

```cs
using System.ComponentModel.DataAnnotations;

public class WeatherForecastDto
{
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Summary must be between 3 and 20 characters.")]
    public string? Summary { get; set; }

    public int TemperatureF { get; set; }

    public String? Alert { get; set; }
    public ICollection<String>? Comments { get; set; }
    public ICollection<int>? CityIds { get; set; }
}

```

```cs
using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class CityForecastService
    {
        private IReadRepository<int, City> cityRepository;
        private IWriteRepository<int, CityWeatherForecast> cityWeatherForecastRepository;

        public CityForecastService(IReadRepository<int, City> cityRepository, IWriteRepository<int, CityWeatherForecast> cityWeatherForecastRepository)
        {
            this.cityRepository = cityRepository;
            this.cityWeatherForecastRepository = cityWeatherForecastRepository;
        }

        public  WeatherForecast Associate(WeatherForecast weatherForecast, ICollection<int> cityIds)
        {
            if(cityIds != null)
            {
                foreach (int cityId in cityIds)
                {
                    City city = cityRepository.FindById(cityId);
                    CityWeatherForecast cityWeatherForecast = new();
                    cityWeatherForecast.CityId = cityId;
                    cityWeatherForecast.City = city;
                    cityWeatherForecast.WeatherForecastId = weatherForecast.Id;
                    cityWeatherForecast.WeatherForecast = weatherForecast;

                    cityWeatherForecastRepository.Save(cityWeatherForecast);
                }
            }
            
            return weatherForecast;
        }
    }
}
```

```cs
namespace MyFirstApi.Models
{
    using Microsoft.EntityFrameworkCore;

    public class CityWeatherForecastRepository : IWriteRepository<int, CityWeatherForecast>, IReadRepository<int, CityWeatherForecast>
    {
        private WeatherForecastDbContext context;

        public CityWeatherForecastRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

        public CityWeatherForecast Save(CityWeatherForecast cityWeatherForecast)
        {
            context.CityWeatherForecasts.Add(cityWeatherForecast);
            context.SaveChanges();
            return cityWeatherForecast;
        }

        public CityWeatherForecast Update(int id, CityWeatherForecast cityWeatherForecast)
        {
            cityWeatherForecast.Id = id;
            context.CityWeatherForecasts.Update(cityWeatherForecast);
            context.SaveChanges();
            return cityWeatherForecast;
        }

        public CityWeatherForecast RemoveById(int id)
        {
            var cityWeatherForecast = context.CityWeatherForecasts.Find(id);
            context.CityWeatherForecasts.Remove(cityWeatherForecast);
            context.SaveChanges();
            return cityWeatherForecast;
        }

        public List<CityWeatherForecast> FindAll()
        {
            return context.CityWeatherForecasts
            .ToList();
        }

        public CityWeatherForecast FindById(int id)
        {
            return context.CityWeatherForecasts.Find(id);
        }

        public int Count()
        {
            return context.CityWeatherForecasts.Count();
        }
    }
}
```

```cs
namespace MyFirstApi.Models
{
    using Microsoft.EntityFrameworkCore;

    public class WeatherForecastRepository : IWriteRepository<int, WeatherForecast>, IReadRepository<int, WeatherForecast>
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
    }
}
```

```cs
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;
using MyFirstApi.Services;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<WeatherForecastRepository>();
builder.Services.AddScoped<IReadRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddScoped<IWriteRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());

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
dotnet ef migrations add AddCityWeatherForecastTables
```

```
dotnet run
```

```json
POST http://localhost:5227/admin/city
{
    "Name": "Chicago"
}

POST http://localhost:5227/admin/city
{
    "Name": "New York"
}
```

```json
POST http://localhost:5227/admin/weatherforecast

{
    "Date": "2024-11-24",
    "TemperatureF": 60,
    "Summary": "Pretty Nice",
    "Alert": "Thunderstorm Warning",
    "Comments": [
        "Loving the weather",
        "I wish it were warmer :)"
    ],
    "CityIds": [1,2]
}
```

In the coding exercise you will create a many to many relationship.

## Main Points
- A many-to-many relationship is when many entity instances from one table are associated with many entity instances in another table.
- This means that both tables are linked through a thrid table that holds a foreign key relationship with both of the related tables.

## Suggested Coding Exercise
- You could associate people with each other as parents and children.

## Building toward CSTA Standards:
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects (3A-AP-17) https://www.csteachers.org/page/standards
- Create artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs (3A-AP-18) https://www.csteachers.org/page/standards
- Compare and contrast fundamental data structures and their uses (3B-AP-12) https://www.csteachers.org/page/standards
- Construct solutions to problems using student-created components, such as procedures, modules and/or objects (3B-AP-14) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards
- Modify an existing program to add additional functionality and discuss intended and unintended implications (e.g., breaking other functionality) (3B-AP-22) https://www.csteachers.org/page/standards

## Resources
- https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
