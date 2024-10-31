using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static List<WeatherForecast> forecast = new List<WeatherForecast>();

        [HttpGet]
        public IEnumerable<WeatherForecast> ListAllWeatherForecasts()
        {
            return forecast;
        }

        [HttpGet("boom")]
        public IActionResult GoBoom()
        {
            Response.Headers.Append("API-Version", "1.0");
            Response.Headers.Append("Something", "else");
            
            try
            {
                throw new Exception("Boom!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    Message = ex.Message,
                    SupportContact = "support@knightmoves.org"
                });
            }
        }

        [HttpPost]
        public IActionResult CreateWeatherForecast([FromBody] WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return Created($"/weatherforecast/{forecast.Count - 1}", weatherForecast);
        }

        [HttpGet("{id}")]
        public IActionResult FindById(int id)
        {
            if (id > (forecast.Count - 1))
            {
                return NotFound();
            }
            return Ok(forecast[id]);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateById([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            if (id > (forecast.Count - 1))
            {
                return NotFound();
            }
            forecast[id] = weatherForecast;
            return Ok(weatherForecast);
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
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