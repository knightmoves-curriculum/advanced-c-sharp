In today's lesson we'll learn how to increase the separation between the controller and the repository using a combination of design patterns.
Right now the Controller creates a new repository on line 10.  Let's remove this creation knowledge from the controller step-by-step.




``` cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using MyFirstApi.Controllers;
using MyFirstApi.Models;

namespace MyFirstApi
{
    public class ApplicationFactory : IControllerFactory
    {
        private static WeatherForecastRepository repository;

        public object CreateController(ControllerContext context)
        {
            Console.WriteLine("setting up controller...");

            if(repository == null){
                Console.WriteLine("setting up singleton repository");
                repository = new WeatherForecastRepository();
            }

            return new WeatherForecastController(repository);
        }

        public void ReleaseController(ControllerContext context, object controller)
        {
            if (controller is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
```

This is using the Factory Method pattern.  The Factory Method pattern is a design pattern that uses factory methods to deal with the problem of creating objects.

The repository is only created once in order remove the need for a static forecast in our repository.  While the controller should be created each time we only want a single instance of the repository.  
Limiting the creation of a class to only one instance is called the Singleton Pattern.

``` cs
using Microsoft.AspNetCore.Mvc.Controllers;
using MyFirstApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IControllerFactory, ApplicationFactory>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

```

Now let's test it out by hand to make sure everything is working.

Now the controller does not create it's own Repository instead the factory is injecting a repository into the controller.  This is called Dependency Injection.
Dependency Injection (DI) is a method of providing a class with its necessary dependencies from the outside rather than having the class create them itself. This promotes cleaner code by reducing direct connections between classes, making them easier to test and maintain.
The Controller no longer knows how to create a new repository.  Instead it requires a repository in it's constructor and couldn't care less how it gets there.

This is a partial example of the Dependency Inversion Principle.  The Dependency Inversion is a principle in software design that states high-level modules should not depend on low-level modules; both should depend on abstractions. While the high-level controller no longer creates a new low-levl repository it still requires a concrete class in its constructor.  In order to truly follow the Dependency Inversion Principle we need to separate the controller from the repository through an interface or some other abstraction.

``` cs
using System.Collections.Generic;

namespace MyFirstApi.Models
{
    public interface IRepository<TId, T>
    {
        T Save(T entity);
        T Update(TId id, T entity);
        List<T> FindAll();
        T FindById(TId id);
        T RemoveById(TId id);
    }
}
```

In the coding exercise, you will apply the dependency inversion principle.

## Main Points
- The Factory Method pattern is a design pattern that uses factory methods to deal with the problem of creating objects.
- The Singleton Pattern limits the creation of a class to only one instance
- Dependency Injection (DI) is a method of providing a class with its necessary dependencies from the outside rather than having the class create them itself.
- The Dependency Inversion is a principle in software design that states high-level modules should not depend on low-level modules; both should depend on abstractions.

## Suggested Coding Exercise
You may want to do all the setup around the factory and stuff and have it new up a controller that still creates a respository inside the controller.
Then have them create an interface, inject the interface through the constructor and update the factory to create a new repository that implements the interface and injects it through the controller's constructor.

## Building toward CSTA Standards:
- Explain how abstractions hide the underlying implementation details of computing systems embedded in everyday objects. (3A-CS-01) https://www.csteachers.org/page/standards
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects. (3A-AP-17) https://www.csteachers.org/page/standards
- Construct solutions to problems using student-created components, such as procedures, modules and/or objects. (3B-AP-14) https://www.csteachers.org/page/standards
- Compare levels of abstraction and interactions between application software, system software, and hardware layers. (3A-CS-02) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Factory_(object-oriented_programming)
- https://en.wikipedia.org/wiki/Singleton_pattern
- https://en.wikipedia.org/wiki/Dependency_injection
- https://en.wikipedia.org/wiki/Dependency_inversion_principle
