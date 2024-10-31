In today's lesson we'll refactor our controller.  One guiding principle that you should always keep in mind is the Single Responsibility Principle.  
The Single Responsibility Principle states that a class should have only one reason to change, meaning it should focus on a single task or responsibility. 
This keeps code modular and easier to maintain, so that changes in one part of the application won’t impact unrelated functionality.

In order to understand a class' responsibility we must understand it's purpose.  The WeatherForecastController is a controller.  
A controller is like a waiter in a restraunt that takes orders from the customer to the chef and the food from the chef to the customer.  
The controller is a go between.  In this restraunt anology the customer is the view and the chef is the model.  
In software this "restraunt" pattern is called the MVC pattern or the Model-View-Controller pattern.  
In our API the view is the JSON that we are returning and the model is the WeatherForecast itself.

With this in mind, it starts to become clearer that the controller has at least two responsibilities.  
Beyond it's duties as a waiter it is also responsible for storing the forecast.  Managing the forecast should be owned by the chef not the waiter.  
Let's pull this out of the controller into the model layer.

``` cs
namespace MyFirstApi.Models
{
    public class WeatherForecastRepository
    {
        private List<WeatherForecast> forecast;

        public WeatherForecastRepository()
        {
            forecast = new List<WeatherForecast>();
        }

        public WeatherForecast Save(WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return weatherForecast;
        }

        public WeatherForecast Update(int id, WeatherForecast weatherForecast)
        {
            forecast[id] = weatherForecast;
            return weatherForecast;
        }

        public List<WeatherForecast> FindAll()
        {
            return forecast;
        }

        public WeatherForecast FindById(int id)
        {
            return forecast[id];
        }

        public WeatherForecast RemoveById(int id)
        {
            var weatherForecast = forecast[id];
            forecast.Remove(weatherForecast);
            return weatherForecast;
        }
    }
}
```

``` cs
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static WeatherForecastRepository repository = new WeatherForecastRepository();

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(repository.FindAll());
        }

        [HttpGet("boom")]
        public IActionResult GetBoom()
        {
            Response.Headers.Append("API-Version", "1.0");

            try
            {
                throw new Exception("Boom!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Error = "Internal Server Error",
                    Message = ex.Message,
                    SupportContact = "support@knightmove.org"
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult FindById(int id)
        {
            if (id > (repository.FindAll().Count - 1))
            {
                return NotFound();
            }
            var weatherForecast = repository.FindById(id);
            return Ok(weatherForecast);
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast weatherForecast)
        {
            repository.Save(weatherForecast);
            return Created($"/weatherforecast/{repository.FindAll().Count - 1}", weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            if (id > (repository.FindAll().Count - 1))
            {
                return NotFound();
            }
            repository.Update(id, weatherForecast);
            return Ok(weatherForecast);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id > (repository.FindAll().Count - 1))
            {
                return NotFound();
            }
            var weatherForecast = repository.RemoveById(id);
            return Ok(weatherForecast);
        }
    }
}
```

By pulling out this concern we've encapsulated forecast management inside a separate class.  Encapsulation is the practice of bundling data with its related methods and restricting direct access to protect the object's internal state.

In the coding exercise, you will apply the Single Responsibility Principle.

## Main Points
- The Single Responsibility Principle states that a class should have only one reason to change
- MVC = Model View Controller
- Encapsulation is the practice of bundling data with its related methods and restricting direct access to protect the object's internal state.

## Suggested Coding Exercise
Extract person storage to a repository class.

## Building toward CSTA Standards:
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects. (3A-AP-17) https://www.csteachers.org/page/standards
- Create artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs. (3A-AP-18) https://www.csteachers.org/page/standards
- Construct solutions to problems using student-created components, such as procedures, modules and/or objects. (3B-AP-14) https://www.csteachers.org/page/standards
- Compare levels of abstraction and interactions between application software, system software, and hardware layers. (3A-CS-02) https://www.csteachers.org/page/standards
- Evaluate the tradeoffs in how data elements are organized and where data is stored. (3A-DA-10) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Single-responsibility_principle
- https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller
