In today's lesson we'll gracefully look at how APIs deal with exceptions that are thrown inside code.  We will also look at a way to customize our responses.  
If our code encounteres an unexpected exception it will return a 500, Internal Server Error response status code.

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

        [HttpGet("boom")]
        public IActionResult GetBoom()
        {
            Response.Headers.Add("API-Version", "1.0");

            try{
                throw new Exception("Boom!");
            } catch (Exception ex){
                return StatusCode(500, new
                {
                    Status = 500,
                    Error = "Internal Server Error",
                    Message = ex.Message,
                    SupportContact = "support@knightmove.org"
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult FindById(int id)
        {
            if(id > (forecast.Count - 1))
            {
                return NotFound();
            }
            var weatherForecast = forecast[id];
            return Ok(weatherForecast);
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return Created($"/weatherforecast/{forecast.Count - 1}", weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            if(id > (forecast.Count - 1))
            {
                return NotFound();
            }
            forecast[id] = weatherForecast;
            return Ok(weatherForecast);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if(id > (forecast.Count - 1))
            {
                return NotFound();
            }
            var weatherForecast = forecast[id];
            forecast.Remove(weatherForecast);
            return Ok(weatherForecast);
        }
    }
}
```
Allowing a raw exception to be displayed to the client is very dangerous.  Form a single error stack anyone can learn alot about your application and find ways to exploit it's vulnerabilities.
Instead we should catch the error and return a 500 with a custom response.  

In the coding exercise, you will use a custom Http response.

## Main Points

## Suggested Coding Exercise

## Building toward CSTA Standards:

## Resources
