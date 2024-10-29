In today's lesson we'll delete a weather forecast.  Up until now we've created weather forecasts using the HTTP POST method. 
We've read weather forecasts using the HTTP GET method. And we've updated weather forecasts using the HTTP PUT method.  
In today's lecture we will delete weather forecasts using the HTTP DELETE method.

Let's add a new DELETE endpoint to our controller.

``` cs
using Microsoft.AspNetCore.Mvc;
using System;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController: ControllerBase
    {
        private static List<WeatherForecast> forecast = new List<WeatherForecast>();

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return forecast;
        }

        [HttpGet("{category}/{id}")]
        public WeatherForecast FindById(int sids, [FromRoute(Name= "category")] string thing, int identifier)
        {
            Console.WriteLine("something " + thing);
            var weatherForecast = forecast[sids];
            return weatherForecast;
        }

        [HttpPost]
        public WeatherForecast Post([FromBody] WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return weatherForecast;
        }

        [HttpPut("{id}")]
        public WeatherForecast Put([FromBody] WeatherForecast weatherForecast, [FromRoute] int id)
        {
            forecast[id] = weatherForecast;
            return weatherForecast;
        }

        [HttpDelete("{id}")]
        public WeatherForecast Delete(int id)
        {
            var weatherForecast = forecast[id];
            forecast.Remove(weatherForecast);
            return weatherForecast;
        }
    }
}
```
Now we are able to create, read, update and delete weather forecasts.  The way in which we built this Weather Forecast API is called REST.  REST stands for Representational State Transfer.  
In the coding exercise, you will create a DELETE endpoint. REST is a set of rules and conventions for building and interacting with a specific type of web application called a services. 
A service is a part of an application that performs a specific job or task, often to help organize code and make it easier to reuse and maintain.
REST APIs allow different software systems to communicate over the web using HTTP.  When you build a REST API keep the following concepts in mind.
1. In a REST API everything is considered a "resource".  In our case the resource is a weather forecast.
2. These resources should be created, read, updated and deleted using their corresponding HTTP methods: POST, GET, PUT, and DELETE.
3. REST APIs are stateless.  Statelessness means that each time a client makes a request, the server treats it as a new, independent request without remembering past interactions. This makes the server simpler and faster, as it doesn’t need to keep track of who made which request.
4. REST APIs typically return JSON as it's lightweight and easy for humans and machines to read.
5. REST APIs use standardized URLs and Responses.  RESTful design involves standardized URLs for resources.  In the case of the weather forecast we see that every url starts with the resource or if we think about it like a sentence it is the noun.  The HTTP methods are the verbs: create, read, update and delete.  This format makes it easier for humans to discover and understand what types of nouns and verbs a REST API provides.  In addition to the url request, REST APIs use clear response codes to make interactions predictable and easy to use.  We will look at response codes in another lecture.

## Main Points
- The DELETE method is used to remove an existing resource.
- REST = Representational State Transfer
- REST is a set of rules and conventions for building and interacting with a specific type of web application called a services.
- REST APIs are stateless.
- POST creates a resource
- GET reads a resource
- PUT updates a resource
- DELETE removes a resource

## Suggested Coding Exercise
- Have them delete a person.

## Building toward CSTA Standards:
- Create artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs (3A-AP-18) https://www.csteachers.org/page/standards
- Create computational models that represent the relationships among different elements of data collected from a phenomenon or process (3A-DA-12) https://www.csteachers.org/page/standards
- Explain how abstractions hide the underlying implementation details of computing systems embedded in everyday objects (3A-CS-01) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/REST
- https://en.wikipedia.org/wiki/HTTP
- https://en.wikipedia.org/wiki/HTTP#Request_methods
