In today's lesson we'll find a weather forecast by id.

In order to accomplish this we want to add the id to the end of the weather forecast url

`/weatherforecast/1`

In order to accomplish this we will use a route template. A route template is a parameterized route that defines how to create routes from input parameters.
Route templates contain one or more route parameters that are defined between two curly braces or mustaches.

`/weatherforecast/{id}`

As long as the name of the method parameter matches the name of the route parameter the value will be extracted from the url and passed to the method.

`/weatherforecast/{category}/{id}`

Multiple parameters can be passed in the same url as long as they are separated by a literal value like a forward slash.


You can also add the FromRoute attribute to one or more method parameters.

If your getting tired of restarting your application server to pick up new changes try using 
`dotnet watch`

This picks up changes and hot deploys your changes.

In the coding exercise, you will create an endpoint with a route template.



## Resources
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-8.0#route-templates
