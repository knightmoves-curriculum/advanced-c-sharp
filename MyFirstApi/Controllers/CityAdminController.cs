using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("admin/city")]
    public class CityAdminController : ControllerBase
    {
        private IWriteRepository<int, City> repository;
        private IMapper mapper;

        public CityAdminController(IWriteRepository<int, City> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpPost]
        public IActionResult Post([FromBody] CityDto cityDto)
        {
            City city = mapper.Map<City>(cityDto);
            repository.Save(city);
            return Created($"/city/{repository.Count()}", city);
        }
    }
}