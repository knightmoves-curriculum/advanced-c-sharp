In today's lesson we'll pull the mapping logic out of the controller in order to satisfy the single responsibility principle.  While we pull this code out we will simplify the mapping prossess by using Automatpper to map our DTO to entities.  AutoMapper is a small library that significantly reduces the code needed to map one object to another.  It's a best practice to use libraries instead of writing your own implementation to save time and take advantage of well-tested, proven code.

```
dotnet add package AutoMapper
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
        private IMapper mapper;

        public WeatherForecastAdminController(IWriteRepository<int, WeatherForecast> repository, 
                                                CurrentWeatherForecastService currentWeatherForecastService,
                                                IMapper mapper)
        {
            this.repository = repository;
            this.currentWeatherForecastService = currentWeatherForecastService;
            this.mapper = mapper;
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecastDto weatherForecastDto)
        {
            WeatherForecast weatherForecast = mapper.Map<WeatherForecast>(weatherForecastDto);
            repository.Save(weatherForecast);
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
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;
using MyFirstApi.Services;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<WeatherForecastRepository>();
builder.Services.AddScoped<IReadRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddScoped<IWriteRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());

builder.Services.AddTransient<CurrentWeatherForecastService>();
builder.Services.AddHttpClient<CurrentWeatherForecastService>();

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
In the coding exercise you will use AutoMapper.

## Main Points
- AutoMapper is a small library that significantly reduces the code needed to map one object to another.
- It's a best practice to use libraries instead of writing your own implementation to save time and take advantage of well-tested, proven code.

## Suggested Coding Exercise
- Cut over to AutoMapper

## Building toward CSTA Standards:
- Create prototypes that use algorithms to solve computational problems by leveraging prior student knowledge and personal interests (3A-AP-13) https://www.csteachers.org/page/standards
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects (3A-AP-17) https://www.csteachers.org/page/standards
- Create artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs (3A-AP-18) https://www.csteachers.org/page/standards
- Use and adapt classic algorithms to solve computational problems (3B-AP-10) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards

## Resources
- https://github.com/AutoMapper/AutoMapper
