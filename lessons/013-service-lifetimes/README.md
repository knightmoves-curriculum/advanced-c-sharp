In today's lesson we'll look at how ASP.NET Core supports dependency injection out of the box.  In the previous lesson we created our own factory and injected the repository into the controller.
While that was helpful in understanding how dependency injection works, we don't have to do this by hand.  ASP.NET has a built-in dependency injection container. This dependency injection container, also known as the IoC Continer or an Inversion of Control container, manages the creation and injection of classes.  The Inversion of Control (IoC) principle is a design approach in which the control of object creation and dependency management is delegated from the object itself to an external entity, often a DI container, like ASP.NET.  While this may seem difficult to understand you'll quickly see how much easier and cleaner it is to just let ASP.NET do this for you.

Singleton lifetime ensures that a single instance of a service is created and shared across the entire application for all requests and users, lasting until the application is restarted.
Transient lifetime services are created each time they're requested from the service container.
For web applications, a scoped lifetime indicates that services are created once per client request (connection).

``` cs
using MyFirstApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IRepository<int, WeatherForecast>, WeatherForecastRepository>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

app.Run();
```

In a web application, singleton services must be stateless, meaning they don't store any information specific to a single user.  Since there is only one instance of a singleton within an application if it did store, for instance bank information, then one user could access another user's bank information.  This is why it's so important to understand that singleton services must be stateless.

In the coding exercise, you will use ASP.NET to inject a service.

## Main Points
- ASP.NET has a built-in dependency injection container.
- Dependency Injections container is also known as an Inversion of Control container.
- The Inversion of Control (IoC) principle is a design approach in which the control of object creation and dependency management is delegated from the object itself to an external entity, often a DI container, like ASP.NET.
- Singleton lifetime ensures that a single instance of a service is created and shared across the entire application for all requests and users, lasting until the application is restarted.
- Transient lifetime services are created each time they're requested from the service container.
- For web applications, a scoped lifetime indicates that services are created once per client request (connection).

## Suggested Coding Exercise
- Cut over to use ASP.NET to inject the repository into the controller

## Building toward CSTA Standards:
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects. (3A-AP-17) https://www.csteachers.org/page/standards
- Construct solutions to problems using student-created components, such as procedures, modules and/or objects. (3B-AP-14) https://www.csteachers.org/page/standards
- Compare levels of abstraction and interactions between application software, system software, and hardware layers. (3A-CS-02) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs. (3B-AP-16) https://www.csteachers.org/page/standards

## Resources
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0#service-lifetimes
- https://en.wikipedia.org/wiki/Inversion_of_control
