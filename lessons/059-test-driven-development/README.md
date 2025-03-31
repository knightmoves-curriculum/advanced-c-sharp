In today's lesson we'll look at Test Driven Development. Test-Driven Development (TDD) is a software development process where tests are written before the actual implementation exists. It follows a cycle of Red (fail) → Green (pass) → Refactor (improve). It works by first writing a failing test case, then implementing just enough code to pass the test, and finally refactoring the code while ensuring all tests continue to pass. TDD is valuable because it ensures code correctness, improves design by encouraging modularity, and provides a safety net for refactoring, leading to more maintainable and reliable software.

``` cs
using System.Net;
using MyFirstApi.Pagination;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
public class CityAdminControllerTest : UserAcceptanceTest
{
    private readonly CityDto _cityDto;

    public CityAdminControllerTest(WebApplicationFactory<Program> factory): base(factory, "Admin")
    {
        var result = new PaginatedResult<WeatherForecast>();
        result.Items = new List<WeatherForecast>();
        _cityDto = new CityDto
        {
            Name = "testCityName"
        };
    }

    [Fact]
    public async Task ShouldCreateCity_WhenGivenValidCity()
    {
        var response = await _client.PostAsJsonAsync("/admin/City", _cityDto);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdCity = await response.Content.ReadFromJsonAsync<City>();
        Assert.NotNull(createdCity);
        Assert.True(createdCity.Id > 0);
        Assert.Equal(_cityDto.Name, createdCity.Name);
    }

    [Fact]
    public async Task ShouldDeleteCity_WhenGivenValidCityId()
    {
        var initialResponse = await _client.PostAsJsonAsync("/admin/City", _cityDto);
        Assert.Equal(HttpStatusCode.Created, initialResponse.StatusCode);
        var createdCity = await initialResponse.Content.ReadFromJsonAsync<City>();
        Assert.NotNull(createdCity);
        var cityId = createdCity.Id;

        var response = await _client.DeleteAsync("/admin/City/" + cityId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var deletedCity = await response.Content.ReadFromJsonAsync<City>();
        Assert.NotNull(deletedCity);
        Assert.Equal(deletedCity.Id, cityId);
    }

    [Fact]
    public async Task ShouldNotDeleteCity_WhenGivenInvalidCityId()
    {
        var initialResponse = await _client.PostAsJsonAsync("/admin/City", _cityDto);
        Assert.Equal(HttpStatusCode.Created, initialResponse.StatusCode);
        var createdCity = await initialResponse.Content.ReadFromJsonAsync<City>();
        Assert.NotNull(createdCity);
        var cityId = createdCity.Id;
        var invalidCityId = cityId + 100;

        var response = await _client.DeleteAsync("/admin/City/" + invalidCityId);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
```

``` cs
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

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try{
                var city = repository.RemoveById(id);
                return Ok(city);
            } catch (Exception){
                return NotFound();
            }
        }
    }
}
```

`dotnet test`

In the coding exercise you will test drive your code.

## Main Points
- Test-Driven Development (TDD) is a software development process where tests are written before the actual implementation exists. 
- Test-Driven-Development follows a cycle of Red (fail) → Green (pass) → Refactor (improve).


## Suggested Coding Exercise
- Have students test drive their code.  Have them commit after each step:
    - Red test commit
    - Green commit

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
- https://en.wikipedia.org/wiki/Test-driven_development
