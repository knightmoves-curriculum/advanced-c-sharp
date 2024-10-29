In today's lesson we'll delete a weather forecast.  Up until now we've created weather forecasts using the HTTP POST method. 
We've read weather forecasts using the HTTP GET method. And we've updated weather forecasts using the HTTP PUT method.  
In today's lecture we will delete weather forecasts using the HTTP DELETE method.

Let's add a new DELETE endpoint to our controller.

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

        [HttpPut("{id}")]
        public WeatherForecast Put([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            forecast[id] = weatherForecast;
            return weatherForecast;
        }

        [HttpDelete("{id}")]
        public WeatherForecast Delete(int id)
        {
            var weatherForecast = forecast[id];
            forecast.Remove(weatherForecast);
            return weatherForecast;
        }
    }
}
```

In the coding exercise, you will create a DELETE endpoint.

## Main Points
- The PUT method is used to update an existing resource with new information.

## Suggested Coding Exercise
- Have them update the person info.

## Building toward CSTA Standards:
- Explain how abstractions hide the underlying implementation details of computing systems embedded in everyday objects (3A-CS-01) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/REST
- https://en.wikipedia.org/wiki/HTTP
- https://en.wikipedia.org/wiki/HTTP#Request_methods
