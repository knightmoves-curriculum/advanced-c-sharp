using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("admin/weatherforecast")]
    public class WeatherForecastAdminController : ControllerBase
    {
        private IWriteRepository<int, WeatherForecast> repository;

        public WeatherForecastAdminController(IWriteRepository<int, WeatherForecast> weatherForecastRepository)
        {
            Console.WriteLine("contructing WeatherForecastController...");
            repository = weatherForecastRepository;
        }

        [HttpPost]
        public IActionResult CreateWeatherForecast([FromBody] WeatherForecast weatherForecast)
        {
            repository.Save(weatherForecast);
            return Created($"/weatherforecast/{repository.Count() - 1}", weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateById([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            if (id > (repository.Count() - 1))
            {
                return NotFound();
            }
            repository.Update(id, weatherForecast);
            return Ok(weatherForecast);
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            if (id > (repository.Count() - 1))
            {
                return NotFound();
            }
            WeatherForecast weatherForecast = repository.RemoveById(id);
            return Ok(weatherForecast);
        }
    }
}