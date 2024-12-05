using Microsoft.AspNetCore.Mvc;

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
        public IEnumerable<WeatherForecast> FindById(int id)
        {
            var weatherForecast = forecast[id];
            return forecast;
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return Created($"/weatherforecast/{forecast.Count - 1}", weatherForecast);
        }

        [HttpPut("{id}")]
        public WeatherForecast Update([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
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