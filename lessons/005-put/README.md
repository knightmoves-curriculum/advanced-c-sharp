In today's lesson we'll update a weather forecast.  Up until now we've listed all, created and returned a signle weather forecast by id.  
To accomplish these 3 actions we've used two HTTP methods: GET and POST.  The GET method is used to read weather forecasts.  
This method is used to list all and list by id.  The POST method is used to create a new weather forecast.  
In today's lecture we will add one more HTTP method called PUT.  The PUT method is used to update an existing weather forecast with new information.

Let's add a new PUT endpoint to our controller.

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
    }
}
```

In the coding exercise, you will create a PUT endpoint.

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
