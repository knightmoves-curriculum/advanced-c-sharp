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
    }
}