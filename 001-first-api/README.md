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

- /obj: This folder contains intermediate files generated during the build process.
- /Properties: This folder contains project-specific settings and properties. 
- appsettings.json: This is the main configuration file for the application. It can store various settings in JSON format, such as database connection strings, logging configurations, and any other application-specific settings.
- MyFirstApi.csproj: This is the project file for your web API application. It contains metadata about the project, such as the project’s name, target framework, dependencies, and build settings. It is essential for the .NET build system.
- Program.cs: This is the main entry point for your application. It contains the Main method, which is where the application starts running. In a typical ASP.NET Core application, this file also configures the web host, sets up services, and configures the application pipeline.

Now let's cd, or change directoreis, into the `MyFirstApi` directory and run `dotnet run`

After a few moments, you should see output indicating that the application is running.  
In a browser let's navigate to http://localhost:5000/weatherforecast

The information that is returned is formatted as JSON or JavaScript Object Notation.
JSON is a lightweight data interchange format that is easy for humans to read and write and easy for machines to parse and generate.
Despite its name, it is not directly tied to JavaScript and is supported by a variety of programming languages.
It is commonly used in APIs to send and receive data because it provides a simple and structured way to represent complex data objects.

In order to stop your API hold the control button on your keyboard while you press the `C` button at the same time.

Before you commit your code remember to add a .gitignore file to your project in order to prevent Git from commiting files that don't need to be or shouldn't be pushed to GitHub.
A few examples are 
- compiled computer code,
- configured libraries that can be downloaded each time,
- local configuration files
- and local secrets and properties that should not be shared.

```
bin/
obj/
.vscode/
*.secrets.json
```
- The bin/ folder contains the compiled output of your .NET project, including the executable (.exe) or library (.dll) files, and any other binaries generated during the build process. The files in this directory are generated automatically by the build process, so they don't need to be tracked in version control.
- The obj/ folder contains intermediate files that are generated during the build process. This includes compiled resources, temporary files, and metadata used to assist the build process, like .pdb files for debugging and .dll files used during compilation. Like bin/, the contents of the obj/ folder are generated automatically and can be recreated when building the project.
- The .vscode/ folder is used by Visual Studio Code to store user-specific and workspace-specific configuration files, such as custom settings, extensions, debugging configurations, and launch configurations for the project. These settings are specific to your local development environment and may vary significantly between developers.
- Files ending with .secrets.json typically store sensitive information, such as API keys, database connection strings, or other credentials required by the application. Sensitive information like credentials should never be committed to version control for security reasons.

In the coding exercise today, you will use the dotnet CLI to create an API. 


## Main Points
1. API = Application Programming Interface
1. APIs often return content in JSON format
1. JSON = JavaScript Object Notation
1. In order to prevent files and folders from being committed into git you should add them to the .gitignore file

## Suggested Coding Exercise
- Just have them use `dotnet new webapi -n MyFirstApi` to create their own app.
- You could either
  - verify a few file names at the root of the project
  - or you could actually start the app in an acceptance test and hit the started app

 ## Building toward CSTA Standards:
 - Create computational models that represent the relationships among different elements of data collected from a phenomenon or process. (3A-DA-12) https://www.csteachers.org/page/standards
 - Explain how the characteristics of the Internet influence the systems built on it. (3A-NI-07) https://www.csteachers.org/page/standards
 - Describe the issues that impact network functionality (e.g., bandwidth, load, delay, topology). (3B-NI-03) https://www.csteachers.org/page/standards
 - Construct solutions to problems using student-created components, such as procedures, modules, and/or objects. (3B-AP-14) https://www.csteachers.org/page/standards
