using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        private CityForecastService cityForecastService;
        private IMapper mapper;

        public WeatherForecastAdminController(IWriteRepository<int, WeatherForecast> repository,
                                                CityForecastService cityForecastService, 
                                                CurrentWeatherForecastService currentWeatherForecastService,
                                                IMapper mapper)
        {
            this.repository = repository;
            this.cityForecastService = cityForecastService;
            this.currentWeatherForecastService = currentWeatherForecastService;
            this.mapper = mapper;
        }
        
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecastDto weatherForecastDto)
        {
            WeatherForecast weatherForecast = mapper.Map<WeatherForecast>(weatherForecastDto);
            repository.Save(weatherForecast);
            cityForecastService.Associate(weatherForecast, weatherForecastDto.CityIds);
            return Created($"/weatherforecast/{repository.Count()}", weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] WeatherForecastDto weatherForecastDto, [FromRoute] int id)
        {
            WeatherForecast weatherForecast = mapper.Map<WeatherForecast>(weatherForecastDto);
            
            if (id > repository.Count())
            {
                return NotFound();
            }
            repository.Update(id, weatherForecast);
            return Ok(weatherForecast);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id > repository.Count())
            {
                return NotFound();
            }
            var weatherForecast = repository.RemoveById(id);
            return Ok(weatherForecast);
        }

        [HttpPost("current")]
        public async Task<IActionResult> Current(){
            WeatherForecast weatherForecast = await currentWeatherForecastService.Report();
            return Ok(weatherForecast);
        }
    }
}