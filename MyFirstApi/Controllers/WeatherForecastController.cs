using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IReadRepository<int, WeatherForecast> repository;
        private IDateQueryable<WeatherForecast> forecastByDateRepository;

        public WeatherForecastController(IReadRepository<int, WeatherForecast> repository, IDateQueryable<WeatherForecast> forecastByDateRepository)
        {
            this.repository = repository;
            this.forecastByDateRepository = forecastByDateRepository;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] DateOnly? date)
        {
            if(date != null)
            {
                return Ok(forecastByDateRepository.FindByDate((DateOnly)date));
            } else {
                return Ok(repository.FindAll()); 
            }
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

        [HttpGet("boom")]
        public IActionResult Boom()
        {
            throw new InvalidOperationException("boom!");
        }
    }
}