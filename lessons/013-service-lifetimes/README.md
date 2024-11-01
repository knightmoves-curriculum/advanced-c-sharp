In today's lesson we'll look at how ASP.NET Core supports dependency injection.  In the previous lesson we created our own factory and injected the repository into the controller.
While that was helpful in understanding how dependency injection works, we don't have to do this by hand.  ASP.NET has a built-in dependency injection container. The DI container, also known as the IoC Continer or an Inversion of Control container, manages the creation and injection of classes.  The Inversion of Control principle simply means that instead of our classes creating it's own classes it relies on an external framework, like ASP.NET, to create classes and inject classes for it.  While this may seem difficult to understand you'll quickly see how much easier and cleaner it is to just let ASP.NET do this for you.



In the coding exercise, you will ...

## Main Points

## Suggested Coding Exercise

## Building toward CSTA Standards:

## Resources
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0#service-lifetimes
- https://en.wikipedia.org/wiki/Inversion_of_control
