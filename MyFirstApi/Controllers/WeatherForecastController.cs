using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IReadRepository<int, WeatherForecast> repository;

        public WeatherForecastController(IReadRepository<int, WeatherForecast> repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(repository.FindAll());
        }

        [HttpGet("{id}")]
        public IActionResult FindById(int id)
        {
            if (id > repository.FindAll().Count)
            {
                return NotFound();
            }
            var weatherForecast = repository.FindById(id);
            return Ok(weatherForecast);
        }
    }
}