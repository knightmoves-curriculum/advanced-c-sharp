using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IRepository<int, WeatherForecast> repository;

        public WeatherForecastController(IRepository<int, WeatherForecast> weatherForecastRepository)
        {
            repository = weatherForecastRepository;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> ListAllWeatherForecasts()
        {
            return repository.FindAll();
        }

        [HttpGet("boom")]
        public IActionResult GoBoom()
        {
            Response.Headers.Append("API-Version", "1.0");
            Response.Headers.Append("Something", "else");
            
            try
            {
                throw new Exception("Boom!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    Message = ex.Message,
                    SupportContact = "support@knightmoves.org"
                });
            }
        }

        [HttpPost]
        public IActionResult CreateWeatherForecast([FromBody] WeatherForecast weatherForecast)
        {
            repository.Save(weatherForecast);
            return Created($"/weatherforecast/{repository.FindAll().Count - 1}", weatherForecast);
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

        [HttpPut("{id}")]
        public IActionResult UpdateById([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            if (id > (repository.FindAll().Count - 1))
            {
                return NotFound();
            }
            repository.Update(id, weatherForecast);
            return Ok(weatherForecast);
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            if (id > (repository.FindAll().Count - 1))
            {
                return NotFound();
            }
            WeatherForecast weatherForecast = repository.RemoveById(id);
            return Ok(weatherForecast);
        }
    }
}