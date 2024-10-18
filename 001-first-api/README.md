# Creating Your First API with .NET
## What is an API?
In today's lesson we will build an API.  
An API (Application Programming Interface) is a set of rules that allows different software applications to communicate with each other. 
Think of it as a waiter in a restaurant: you tell the waiter what you want, and they bring it back to you. 
In software, APIs allow different parts of a program, or even different programs, to interact and exchange data.

The Google Maps API is a great example of an API in action. 
This API allows developers to embed Google Maps into their websites, mobile apps and even in the dashboard of a car.
While each of these applications are written in different languages, and are used on different devices and opperating systems, a centralized API provides the same information to all three applications.
APIs are crucial because they provide a standardized way for different applications and devices to communicate with each other. 

In this section, we will create our first API using the dotnet command-line interface (CLI). 

We will use the command:

`dotnet new webapi -n MyFirstApi`

What Does This Command Do?
- `dotnet`: This is the command-line tool for .NET. It allows you to create, build, run, and publish .NET applications.
- `new`: This subcommand is used to create a new project or file.
- `webapi`: This specifies the type of project we want to create. In this case, we are creating a Web API.
- `-n` MyFirstApi: This option sets the name of our new project to "MyFirstApi."

Now let's run this command. It generates a new Web API project with a predefined structure, saving you a lot of time.

Let's take a look at the folders and files generated for us:

- Controllers/: This folder contains controller classes that handle incoming HTTP requests and send responses back to the client. By default, you'll find a file named WeatherForecastController.cs here, which we can modify to create our own API endpoints.

- Program.cs: This file contains the entry point for our application. It sets up the web server and configures the application.

- Startup.cs: This file is used to configure services and the app's request handling pipeline. It defines how the application behaves and interacts with incoming requests.

- appsettings.json: This file is used to store configuration settings. You can define database connection strings and other app-specific settings here.

- WeatherForecast.cs: This is a model class that represents the data structure for weather forecast data. It includes properties like Date, TemperatureC, Summary, and TemperatureF.

- MyFirstApi.csproj: This is the project file that contains information about the project, including dependencies and build options.

Now let's cd, or change directoreis, into the `MyFirstApi` directory and run `dotnet run`

After a few moments, you should see output indicating that the application is running.  
In a browser let's navigate to http://localhost:5000/weatherforecast

The information that is returned is formatted as JSON or JavaScript Object Notation.
JSON is a lightweight data interchange format that is easy for humans to read and write and easy for machines to parse and generate.
Despite its name, it is not directly tied to JavaScript and is supported by a variety of programming languages.
It is commonly used in APIs to send and receive data because it provides a simple and structured way to represent complex data objects.

Remember .gitignore

```
bin/
obj/
.vscode/
```

In the coding exercise today, you will use the dotnet CLI to create an API.
