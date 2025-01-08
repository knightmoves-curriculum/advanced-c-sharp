In today's lesson we'll learn about data transfer objects.  A Data Transfer Object (DTO) is a simple object that carries data between processes. DTOs are typically used to encapsulate data and send it from one subsystem of an application to another. DTOs are used to deserialize API payloads to ensure that the data structure matches the API contract, reducing coupling between different layers of an application and promoting cleaner code. By using DTOs, you can avoid directly exposing your domain entities, which helps maintain separation of concerns and simplifies validation and transformation of data.

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
}
```

```cs
...

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("admin/weatherforecast")]
    public class WeatherForecastAdminController : ControllerBase
    {
        ...

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecastDto weatherForecastDto)
        {
            WeatherForecast weatherForecast = Map(weatherForecastDto);
            repository.Save(weatherForecast);
            return Created($"/weatherforecast/{repository.Count()}", weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] WeatherForecastDto weatherForecastDto, [FromRoute] int id)
        {
            WeatherForecast weatherForecast = Map(weatherForecastDto);
            
            if (id > repository.Count())
            {
                return NotFound();
            }
            repository.Update(id, weatherForecast);
            return Ok(weatherForecast);
        }

        public WeatherForecast Map(WeatherForecastDto weatherForecastDto){
            WeatherForecast weatherForecast = new(weatherForecastDto.Date, weatherForecastDto.TemperatureF, weatherForecastDto.Summary);

            if(weatherForecastDto.Alert != null){
                WeatherAlert weatherAlert = new();
                weatherAlert.AlertMessage = weatherForecastDto.Alert;
                weatherForecast.Alert = weatherAlert;
            }
            if(weatherForecastDto.Comments != null){
                weatherForecast.Comments = [];
                foreach(String comment in weatherForecastDto.Comments){
                    WeatherComment weatherComment = new();
                    weatherComment.CommentMessage = comment;
                    weatherForecast.Comments.Add(weatherComment);
                }
            }

            return weatherForecast;
        }

        ...
    }
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

In the coding exercise ...

## Main Points
- 

## Suggested Coding Exercise
- 

## Building toward CSTA Standards:
- 

## Resources
- 

