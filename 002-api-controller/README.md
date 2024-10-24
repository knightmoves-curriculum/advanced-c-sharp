# Creating GET Endpoint Using a Controller

In today's lesson we are going to rearrange the code inside the API that we generated last time.  

- Open Program.cs
  
The Program.cs file sets up your ASP.NET Core web API by creating an application builder and configuring how the app responds to requests. It defines an endpoint for the weather forecast, allowing users to access weather data when they visit the /weatherforecast route, and then runs the application to listen for those requests.  ASP.NET is a web development framework created by Microsoft that allows developers to build dynamic websites and APIs.

It's a best practice to move the MapGet for the weather forecast into a separate file called a controller. This will keep Program.cs clean and focused on setting up the app, while the controller handles specific tasks like responding to requests. As your app grows, separate controller classes make it easier to organize your code and add new features without cluttering the Program file. Furthermore, this structure is more standard in professional projects.

- Remove the following lines
``` cs
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

```cs
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
```

- Create the `/Controllers` folder at the root of the project
- Create the `WeatherForecastController.cs` inside the `/Controllers` folder

``` cs
namespace MyFirstApi.Controllers
{
    public class WeatherForecastController
    {
        
    }
}
```

- Copy the following and paste it into your new controller

```cs
using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController: ControllerBase
    {
        private string[] summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var forecast =  Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    }
}
```

- Clean up the following

```cs
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
```

- Make the record in WeatherForecast in `Program.cs` to `public`

- `dotnet run`
- http://localhost:5227/weatherforecast

So let's walk through the Controller in order to understand what we just wrote.
- `using Microsoft.AspNetCore.Mvc;` This line imports the Microsoft.AspNetCore.Mvc namespace
- `namespace MyFirstApi.Controllers` This defines a new namespace called MyFirstApi.Controllers. Namespaces are used to organize code and prevent naming conflicts. 
- `[ApiController]` This attribute indicates that the class is an API controller. 
- `[Route("[controller]")]` This attribute defines the route for the controller. The placeholder [controller] will be replaced by the name of the controller, minus the "Controller" suffix. So, for this class, the route will be WeatherForecast.
- `public class WeatherForecastController : ControllerBase` This line declares a public class named WeatherForecastController that inherits from ControllerBase. Inheriting from ControllerBase provides methods and properties for handling web calls.
- `[HttpGet]` This attribute indicates that the following method will respond to HTTP GET requests.

``` cs
using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController: ControllerBase
    {
        private string[] summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var forecast =  Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    }
}
```

``` cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

## Main Points
1. ASP.NET is a web development framework created by Microsoft that allows developers to build dynamic websites and APIs.
2. `[ApiController]` This attribute indicates that the class is an API controller. 
3. `[Route("[controller]")]` attribute the placeholder [controller] will be replaced by the name of the controller, minus the "Controller" suffix.
4. `[HttpGet]` This attribute indicates that the following method will respond to HTTP GET requests.

## Suggested Coding Exercise
- Just have them do the same refactoring to a controller
- You could either
  - verify that the controller exists with the correct attributes and content
  - or you could actually start the app in an acceptance test and hit the started app but you would need to verify that the Program.cs does not still have the same content in order to prove the controller is being used.

