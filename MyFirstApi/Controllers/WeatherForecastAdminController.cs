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
        public IActionResult Post([FromBody] WeatherForecastDto weatherForecastDto)
        {
            WeatherForecast weatherForecast = Map(weatherForecastDto);
            repository.Save(weatherForecast);
            return Created($"/weatherforecast/{repository.Count()}", weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] WeatherForecastDto weatherForecastDto, [FromRoute] int id)
        {
            WeatherForecast weatherForecast = Map(weatherForecastDto);
            
            if (id > repository.Count())
            {
                return NotFound();
            }
            repository.Update(id, weatherForecast);
            return Ok(weatherForecast);
        }

        public WeatherForecast Map(WeatherForecastDto weatherForecastDto){
            WeatherForecast weatherForecast = new(weatherForecastDto.Date, weatherForecastDto.TemperatureF, weatherForecastDto.Summary);

            if(weatherForecastDto.Alert != null){
                WeatherAlert weatherAlert = new();
                weatherAlert.AlertMessage = weatherForecastDto.Alert;
                weatherForecast.Alert = weatherAlert;
            }
            if(weatherForecastDto.Comments != null){
                weatherForecast.Comments = [];
                foreach(String comment in weatherForecastDto.Comments){
                    WeatherComment weatherComment = new();
                    weatherComment.CommentMessage = comment;
                    weatherForecast.Comments.Add(weatherComment);
                }
            }

            return weatherForecast;
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