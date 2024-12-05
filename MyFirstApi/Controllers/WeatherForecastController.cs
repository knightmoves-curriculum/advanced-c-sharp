using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController: ControllerBase
    {
        private static WeatherForecastRepository repository = new WeatherForecastRepository();

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(repository.FindAll());
        }

        [HttpGet("{id}")]
        public IActionResult FindById(int id)
        {
            if(id > (repository.FindAll().Count - 1))
            {
                return NotFound();
            }
            var weatherForecast = repository.FindById(id);
            return Ok(weatherForecast);
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast weatherForecast)
        {
            repository.Save(weatherForecast);
            return Created($"/weatherforecast/{repository.FindAll().Count - 1}", weatherForecast);
        }

        [HttpPut("{id}")]
        public WeatherForecast Update([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            repository.Update(id, weatherForecast);
            return weatherForecast;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if(id > (repository.FindAll().Count - 1))
            {
                return NotFound();
            }
            var weatherForecast = repository.RemoveById(id);
            return Ok(weatherForecast);
        }
    }
}