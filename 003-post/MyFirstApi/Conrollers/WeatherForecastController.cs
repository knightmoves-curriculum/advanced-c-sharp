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
        public WeatherForecast CreateWeatherForecast([FromBody] WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return weatherForecast;
        }

        [HttpGet("{category}/{id}")] 
        public WeatherForecast FindById(string category, int id)
        {
            Console.WriteLine("category: " + category);
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