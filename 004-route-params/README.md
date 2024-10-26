In today's lesson we'll find a weather forecast by id.

In order to accomplish this we want to add the id to the end of the weather forecast url

`/weatherforecast/1`

In order to accomplish this we will use a route template. A route template is a parameterized route that defines how to create routes from input parameters.
Route templates contain one or more route parameters that are defined between two curly brackets or mustaches.

`/weatherforecast/{id}`

As long as the name of the method parameter matches the name of the route parameter the value will be extracted from the url and passed to the method.

`/weatherforecast/{category}/{id}`

Multiple parameters can be passed in the same url as long as they are separated by a literal value like a forward slash.


You can also add the FromRoute attribute to one or more method parameters.

If your getting tired of restarting your application server to pick up new changes try using 
`dotnet watch`

This picks up changes and hot deploys your changes.

``` cs
using Microsoft.AspNetCore.Mvc;
using System;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController: ControllerBase
    {
        private static List<WeatherForecast> forecast = new List<WeatherForecast>();

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return forecast;
        }

        [HttpGet("{category}/{id}")]
        public WeatherForecast FindById(int sids, [FromRoute(Name= "category")] string thing, int identifier)
        {
            Console.WriteLine("something " + thing);
            var weatherForecast = forecast[sids];
            return weatherForecast;
        }

        [HttpPost]
        public WeatherForecast Post([FromBody] WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return weatherForecast;
        }
    }
}
```

In the coding exercise, you will create an endpoint with a route template.

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

## Building toward CSTA Standards:
- Explain how abstractions hide the underlying implementation details of computing systems embedded in everyday objects (3A-CS-01) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards

## Main Points
1. A route template is a parameterized route that defines how to create routes from input parameters.
2. route parameters are defined between two curly brackets or mustaches
3. Multiple parameters can be passed in the same url as long as they are separated by a literal value
4. `dotnet watch` picks up changes and hot deploys them

## Suggested Coding Exercise
- I would have them look up a person by last name.  This will stretch them a bit since I didn't show them how to loop through a list of people and conditionally return a person with a last name.  They will have to bring concepts from the C# course into this problem.

## Building toward CSTA Standards:
- Explain how abstractions hide the underlying implementation details of computing systems embedded in everyday objects (3A-CS-01) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards
- Translate between different bit representations of real-world phenomena, such as characters, numbers, and images (3A-DA-09) https://www.csteachers.org/page/standards
- Use lists to simplify solutions, generalizing computational problems instead of repeatedly using simple variables (3A-AP-14) https://www.csteachers.org/page/standards
- Compare and contrast fundamental data structures and their uses (3B-AP-12) https://www.csteachers.org/page/standards

## Resources
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-8.0#route-templates
- https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-watch
