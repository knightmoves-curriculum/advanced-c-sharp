using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;
using MyFirstApi.Pagination;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IPaginatedReadRepository<int, WeatherForecast> repository;

        public WeatherForecastController(IPaginatedReadRepository<int, WeatherForecast> repository)
        {
            this.repository = repository;
        }

        [Authorize(Policy = "UserOnly")]
        [HttpGet]
        public IActionResult Get([FromQuery] DateOnly? date, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            PaginatedResult<WeatherForecast> paginatedResult;

            if(date != null)
            {
                paginatedResult = repository.FindPaginatedByDate((DateOnly)date, pageNumber, pageSize);
            } else {
                paginatedResult = repository.FindPaginated(pageNumber, pageSize); 
            }

            var nextPageUrl = pageNumber < paginatedResult.TotalPages
                    ? Url.Action(nameof(Get), new { pageNumber = pageNumber + 1, pageSize })
                    : null;

            return Ok(new
                {
                    Forecasts = paginatedResult.Items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = paginatedResult.TotalCount,
                    TotalPages = paginatedResult.TotalPages,
                    NextPage = nextPageUrl
                });
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