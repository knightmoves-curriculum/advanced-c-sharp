using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("admin/weatherforecast")]
    public class WeatherForecastAdminController : ControllerBase
    {
        private IWriteRepository<int, WeatherForecast> repository;
        private CurrentWeatherForecastService currentWeatherForecastService;

        public WeatherForecastAdminController(IWriteRepository<int, WeatherForecast> repository, CurrentWeatherForecastService currentWeatherForecastService)
        {
            this.repository = repository;
            this.currentWeatherForecastService = currentWeatherForecastService;
        }

        [HttpPost]
        public IActionResult CreateWeatherForecast([FromBody] WeatherForecast weatherForecast)
        {
            repository.Save(weatherForecast);
            return Created($"/weatherforecast/{repository.Count()}", weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateById([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            if (id > repository.Count())
            {
                return NotFound();
            }
            repository.Update(id, weatherForecast);
            return Ok(weatherForecast);
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            if (id > repository.Count())
            {
                return NotFound();
            }
            WeatherForecast weatherForecast = repository.RemoveById(id);
            return Ok(weatherForecast);
        }

        [HttpPost("current")]
        public async Task<IActionResult> Current(){
            WeatherForecast weatherForecast = await currentWeatherForecastService.Report();
            return Ok(weatherForecast);
        }
    }
}