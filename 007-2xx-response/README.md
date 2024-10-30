In today's lesson we'll look at HTTP response status codes.  When you communicate with an REST API there are two parts to that communication the request and the response.  
Up until now we've looked at the request URL, body and method.  Now we'll take a look at the response.  In addition to the JSON value that is returned, there is also a response code.
These codes are three digit numbers that include numbers between 100 and 599.  Any response within the 100 range indicates that the response will be informational.
Any response within the 200 range indicates that the request was successful.  By default all ASP.NET controller requests return the standard response of 200 or OK.
Another Http response status code in the 200 range is 201 Created.  This status code fits very well with our POST request.  
Let's look at how we can modify this endpoint to respond that the weather forecast was successfully created.

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
        public IActionResult Get()
        {
            return Ok(forecast);
        }

        [HttpGet("{id}")]
        public WeatherForecast FindById(int id)
        {
            var weatherForecast = forecast[id];
            return weatherForecast;
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return Created($"/weatherforecast/{forecast.Count - 1}", weatherForecast);
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

In the coding exercise, you will add a response status code.

## Main Points
- When you communicate with an REST API there are two parts to that communication the request and the response.
- Any response within the 100 range indicates that the response will be informational.
- Any response within the 200 range indicates that the request was successful.
- By default all ASP.NET controller requests return the standard response of 200 or OK.
- 201 = Created

## Suggested Coding Exercise
- Update the create person to respond with a 201 including the URI location for the newly created person.

## Building toward CSTA Standards:

## Resources
- https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
