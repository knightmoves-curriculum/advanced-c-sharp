using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IReadRepository<int, WeatherForecast> repository;

        public WeatherForecastController(IReadRepository<int, WeatherForecast> weatherForecastRepository)
        {
            Console.WriteLine("contructing WeatherForecastController...");
            repository = weatherForecastRepository;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> ListAllWeatherForecasts()
        {
            return repository.FindAll();
        }

        [HttpGet("{id}")]
        public IActionResult FindById(int id)
        {
            if (id > (repository.FindAll().Count - 1))
            {
                return NotFound();
            }
            return Ok(repository.FindById(id));
        }
    }
}