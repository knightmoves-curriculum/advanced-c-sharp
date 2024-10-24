In today's lesson we will add a weather forecast.  Up until now we've listed randomly generated weather forecasts through the HTTP Get Method.
HTTP, or Hypertext Transfer Protocol, is a way for web browsers and servers to talk to each other, allowing you to request and view websites and other online content.
HTTP has several ways to interact with a web applicaiton.  One of these ways is through a GET request.  A GET request in HTTP is used to ask an application server for data without making any changes to it. 
When you make a GET request, like the one you've seen with the WeatherForecast, you're simply asking the server to send back information in a format like JSON. 
This is the most common way to retrieve data from a server in web applications.

In this lesson we will introduce a second HTTP Method called POST.  A POST request in HTTP is used to send data to a server to create a new resource, in our case this is a weather forecast. 
When you make a POST request, you're providing information about a weather forecast which the server processes and stores as a new entry. 
Unlike a GET request, which only retrieves data, a POST request specifically adds new weather forecast information to the application server.

Let's add a new POST endpoint to our controller.

``` cs
using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController: ControllerBase
    {
        private List<WeatherForecast> forecast = new List<WeatherForecast>();

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return forecast;
        }

        [HttpPost]
        public WeatherForecast Post([FromBody] WeatherForecast weatherForecast)
        {
            forecast.Add(weatherForecast);
            return weatherForecast;
        }
    }
}
```

While web browsers are designed to send GET requests to web servers, out-of-the-box it is not well suited for sending POST requests.  Instead, let's use as tool called cURL.
`cURL` stands for "Client URL," and it is a command-line tool used to transfer data to and from an application server using various protocols, such as HTTP, HTTPS, FTP, and more. 
It allows users to easily make API requests by customizing request methods, headers, and data payloads.

```
curl -X GET http://localhost:5227/weatherforecast

curl -X POST http://localhost:5227/weatherforecast \
-H "Content-Type: application/json" \
-d '{"Date": "2024-10-24", "TemperatureC": 10, "Summary": "Burr" }'

curl http://localhost:5227/weatherforecast
```

The post did not get saved.  In ASP.NET Core, the controller class is instantiated, or created as a new instance, for each HTTP request by default. 
This means that the forecast list, being an instance variable of the controller, is recreated as an empty list each time the controller is instantiated for a new request. 
Therefore, the list doesn't persist between requests.

In order to solve this, for now, we can simply make the forcast list `static`.  As you may recall from the last course, a static class-level variable in C# is a variable that is shared across 
all instances of the class, meaning it retains its value even when no objects of the class are created, unlike the instance-level forecast variable that kept loosing our new forecast.  Static class-level variables are unique to each object and retain values while the application is running.


```
curl -X GET http://localhost:5227/weatherforecast

curl -X POST http://localhost:5227/weatherforecast \
-H "Content-Type: application/json" \
-d '{"Date": "2024-10-24", "TemperatureC": 10, "Summary": "Burr" }'

curl http://localhost:5227/weatherforecast
```

As the list of forecasts grows it will become harder and harder to read.  To make these calls to our API easier to organize, send and read let's use a popular tool called Postman.
Postman is a versatile tool designed for sending and receiving HTTP requests to application servers, making it easier to test and interact with APIs. 
It provides a user-friendly interface for creating various types of requests, such as GET and POST, and allows users to view responses in data formats like JSON. 

Let's install the Postman plugin.  In order to get started with Postman you will need to create a free account.

Within the `WeatherForecast` Postman collection add the variable `baseUrl` and populate it with your unique url within codespaces.


In the coding exercise, you will create a POST endpoint.

## Main Points

## Suggested Coding Exercise


