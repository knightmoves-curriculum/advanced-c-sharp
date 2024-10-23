# Creating GET Endpoint Using a Controller

In today's lesson we are going to rearrange the code inside the API that we generated last time.  

The Program.cs file sets up your ASP.NET Core web API by creating an application builder and configuring how the app responds to requests. It defines an endpoint for the weather forecast, allowing users to access weather data when they visit the /weatherforecast route, and then runs the application to listen for those requests.

It's a best practice to move the MapGet for the weather forecast into a separate file called a controller. This will keep Program.cs clean and focused on setting up the app, while the controller handles specific tasks like responding to requests. As your app grows, separate controller classes make it easier to organize your code and add new features without cluttering the Program file. Furthermore, this structure is more standard in professional projects.

- Create the `/Controllers` folder at the root of the project
- Create the `WeatherForecastController` inside the `/Controllers` folder

``` cs
namespace MyFirstApi.Controllers
{
    public class WeatherForecastController
    {
        
    }
}
```
