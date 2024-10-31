In today's lesson we'll look at how to handle exceptions when they are encountered inside an API.  We will also look at a way to customize our responses.  
If our code encounteres an unexpected exception it will return a 500, Internal Server Error response status code.

``` cs
using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
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
            Response.Headers.Append("API-Version", "1.0");

            try
            {
                throw new Exception("Boom!");
            }
            catch (Exception ex)
            {
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
            if (id > (forecast.Count - 1))
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
            if (id > (forecast.Count - 1))
            {
                return NotFound();
            }
            forecast[id] = weatherForecast;
            return Ok(weatherForecast);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id > (forecast.Count - 1))
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
- The HTTP Response Status Code 500 = Internal Server Error
- Allowing a raw exception to be displayed to the client is very dangerous.

## Suggested Coding Exercise
Add a custom response to the api.

## Building toward CSTA Standards:
- Explain how abstractions hide the underlying implementation details of computing systems embedded in everyday objects (3A-CS-01) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
- https://en.wikipedia.org/wiki/Request%E2%80%93response
- https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.statuscode?view=aspnetcore-8.0
- https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpresponse.headers?view=aspnetcore-8.0#microsoft-aspnetcore-http-httpresponse-headers
