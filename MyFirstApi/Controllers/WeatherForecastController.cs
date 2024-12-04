using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public WeatherForecast Post([FromBody] WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return weatherForecast;
        }
    }
}