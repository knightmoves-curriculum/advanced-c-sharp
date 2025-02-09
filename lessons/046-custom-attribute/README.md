In today's lesson we'll look at custom attributes. A custom attribute in C# is a user-defined class that extends System.Attribute, allowing metadata to be attached to code elements like methods, classes, or properties. It enables additional functionality, such as validation or logging, by being processed at runtime using reflection.

``` cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WeatherAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateTemperatureAttribute : ActionFilterAttribute
    {
        private readonly int _minTemp;
        private readonly int _maxTemp;

        public ValidateTemperatureAttribute(int minTemp, int maxTemp)
        {
            _minTemp = minTemp;
            _maxTemp = maxTemp;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("weatherForecastDto", out var value) &&
                value is WeatherForecastDto weatherForecastDto)
            {
                if (weatherForecastDto.TemperatureF < _minTemp || weatherForecastDto.TemperatureF > _maxTemp)
                {
                    context.Result = new BadRequestObjectResult($"Temperature must be between {_minTemp} and {_maxTemp} degrees.");
                }
            }
        }
    }
}
```

ActionFilterAttribute is a built-in ASP.NET Core filter that allows you to execute custom logic before and after a controller action runs, enabling tasks like validation, logging, or modifying requests and responses.

AttributeUsage is a C# attribute that specifies where and how a custom attribute can be applied, using AttributeTargets to restrict its scope. The different types include Method, Class, Property, Parameter, Field, Assembly, and All, allowing attributes to be applied to specific code elements like methods, classes, or properties.

``` cs
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;
using MyFirstApi.Services;
using WeatherAPI.Attributes;

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
        
        [Authorize(Policy = "AdminOnly")]
        [ValidateTemperature(-30, 50)]
        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecastDto weatherForecastDto)
        {
            WeatherForecast weatherForecast = mapper.Map<WeatherForecast>(weatherForecastDto);
            repository.Save(weatherForecast);
            cityForecastService.Associate(weatherForecast, weatherForecastDto.CityIds);
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


`dotnet run`

In the coding exercise you will create a custom attribute.

## Main Points
- A custom attribute in C# is a user-defined class that extends System.Attribute, allowing metadata to be attached to code elements like methods, classes, or properties.
- ActionFilterAttribute is a built-in ASP.NET Core filter that allows you to execute custom logic before and after a controller action runs, enabling tasks like validation, logging, or modifying requests and responses.

## Suggested Coding Exercise
- Have students create a custom attribute

## Building toward CSTA Standards:
None

## Resources
- https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/reflection-and-attributes/