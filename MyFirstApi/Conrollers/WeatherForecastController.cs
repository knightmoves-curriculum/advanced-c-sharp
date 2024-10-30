using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController: ControllerBase
    {
        private  static List<WeatherForecast> forecast = new List<WeatherForecast>();
        
        [HttpGet]    
        public IEnumerable<WeatherForecast> ListAllWeatherForecasts()
        {
            return forecast;
        }

        [HttpPost]
        public IActionResult CreateWeatherForecast([FromBody] WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return Created($"/weatherforecast/{forecast.Count - 1}", weatherForecast);
        }

        [HttpGet("{id}")] 
        public WeatherForecast FindById(int id)
        {
            return forecast[id];
        }

        [HttpPut("{id}")]
        public WeatherForecast UpdateById([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            forecast[id] = weatherForecast;
            return weatherForecast;
        }

        [HttpDelete("{id}")]
        public WeatherForecast Remove(int id)
        {
            var weatherForecast = forecast[id];
            forecast.Remove(weatherForecast);
            return weatherForecast;
        }
    }
}