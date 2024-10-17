# Advanced OOP - ASP.NET Core REST API Course Topics

1. **Introduction and Setup**
   - Install .NET SDK and create a basic Web API project using the .NET CLI.
   - Overview of the project structure

1. **Creating a GET Endpoint**
   - Adding and customizing a basic GET endpoint.
   - Understanding routing conventions and customizing routes for API endpoints.

1. **Creating a POST Endpoint**
   - Adding functionality to handle POST requests and create new resources.

1. **Implementing a PUT Endpoint**
   - Handling updates to existing resources using the PUT method.

1. **Implementing a DELETE Endpoint**
   - Removing resources with the DELETE method and handling responses.

1. **HTTP Status Codes and Customizing Responses**
   - Customizing HTTP status codes to reflect different outcomes, like success and error states.

1. **Handling Query Parameters in API Requests**
   - Implementing dynamic filtering or searching using query parameters.

1. **Working with Route Parameters**
   - Using route parameters for fetching resources dynamically based on URL values.

1. **Implementing a Service Layer Using Dependency Injection**
    - Refactoring business logic into services and injecting them into controllers.
    - Exploring how DI works in ASP.NET Core and setting up services.

1. **Service Lifetimes: Scoped, Transient, and Singleton**
    - Understanding different service lifetimes and when to use each one.

1. **Working with Task-Based Asynchronous Patterns (TAP)**
    - Deep dive into TAP and its use in improving concurrency in APIs.

1. **Introduction to Entity Framework Core and SQLite Setup**
    - Setting up Entity Framework Core with SQLite as a lightweight local database.

1. **Defining Models and Entity Relationships**
    - Modeling data with entity relationships like one-to-many and many-to-many.

1. **CRUD Operations with Entity Framework Core**
    - Implementing Create, Read, Update, and Delete (CRUD) operations using EF Core.
  
1. **LINQ Introduction and Querying Collections**
   - Demonstrate basic LINQ queries on collections, such as filtering, projecting, and ordering data.

1. **LINQ Aggregation and Reduction Operations**
   - Use LINQ methods like `Aggregate`, `Sum`, `Count`, and `Average` to perform aggregation and reduction on collections.

1. **Model Validation with Data Annotations**
    - Using data annotations to validate input models and ensure data integrity.

1. **Advanced Validation Techniques**
    - Creating custom validation attributes and business rule validation.

1. **Global Error Handling and Exception Filters**
    - Implementing centralized error handling and creating custom exception filters.

1. **Handling ModelState and Validation Errors**
    - Dealing with validation errors and returning meaningful error messages to the client.

1. **Implementing Basic Authentication**
    - Securing endpoints using basic authentication.
    - Basic principles of securing an API, including authentication and authorization.

1. **Securing APIs with JWT (JSON Web Tokens)**
    - Implementing JWT authentication to secure API endpoints in a stateless manner.

1. **Authorization with Policies and Claims**
    - Using policies and claims to enforce complex authorization requirements.

1. **Role-Based Authorization**
    - Restricting access to API endpoints based on user roles.

1. **Handling Sensitive Data Securely**
    - Techniques for securely handling and storing sensitive information.

1. **Designing an API with a Service-Oriented Architecture (SOA)**
    - Exploring service-oriented design principles and how to apply them in API development.

1. **Using DTOs (Data Transfer Objects) in APIs**
    - Separating internal models from external-facing DTOs for better API design.

1. **Mapping Entities to DTOs with AutoMapper**
    - Using AutoMapper to simplify mapping between domain models and DTOs.

1. **Versioning an API**
    - Strategies for implementing version control in a REST API to support backward compatibility.

1. **Pagination and Sorting**
    - Implementing pagination and sorting mechanisms for handling large data sets.

1. **Understanding Delegates and Events**
    - Implementing delegates and events to decouple components in services.

1. **Parallel Programming with PLINQ**
   - Demonstrate how to execute LINQ queries in parallel using Parallel LINQ (PLINQ) for performance gains in CPU-bound tasks.

1. **Anonymous Methods and Lambda Expressions**
   - Introduce anonymous methods, and lambda expressions for creating reusable methods and passing behavior as a parameter.

1. **Func, Action, and Predicate**
   - Learn how to use the `Func<T>`, `Action<T>`, and `Predicate<T>` delegate types to create reusable function pointers.

1. **Advanced Delegate Concepts: Multicast Delegates**
   - Show how to use multicast delegates to chain multiple methods into a single delegate invocation.

1. **Anonymous Functions and Closures**
   - Explore how to create anonymous functions and demonstrate closures, where variables from an outer scope can be accessed inside a lambda.

1. **Nullable Types and the Nullable<T> Structure**
   - Explain the use of nullable value types (`int?`, `bool?`) and how the `Nullable<T>` struct works in C#.
   - Enforcing nullability rules to improve code safety and reduce null reference exceptions.

1. **Null-Coalescing and Null-Conditional Operators**
   - Demonstrate how to handle `null` safely with operators like `??` (null-coalescing) and `?.` (null-conditional).

1. **Interface Default Methods in C# 8**
   - Introduce default interface methods and demonstrate how to provide method implementations in interfaces.

1. **Generics: Creating Generic Classes and Methods**
   - Teach how to create generic classes and methods that can operate on any data type.

1. **Generic Constraints**
   - Explain how to use constraints (`where T : class`, `where T : struct`) to limit the types that can be passed to a generic class or method.

1. **Exception Handling with `try`, `catch`, and `finally`**
   - Review the basic concepts of exception handling and how to properly use `try`, `catch`, and `finally` blocks.

1. **Async Programming with `async` and `await`**
   - Introduce asynchronous programming using `async` and `await` to write non-blocking code.

1. **Task and Task<T>**
   - Explore the `Task` and `Task<T>` types and demonstrate how to use them for asynchronous operations in C#.

1. **Creating and Using Attributes in C#**
   - Show how to define and use custom attributes to annotate and provide metadata for classes and methods.

1. **`IEnumerable` vs `IQueryable`: Differences and Use Cases**
   - Discuss the differences between `IEnumerable` and `IQueryable` and when to use each in C# for data querying.

1. **Introduction to Unit Testing with XUnit**
    - Overview of unit testing principles and setting up basic unit tests with XUnit.

1. **Mocking Dependencies for Unit Tests**
    - Using mocking frameworks like Moq to mock service dependencies in unit tests.

1. **Testing Asynchronous Code**
    - Writing unit tests for asynchronous methods in controllers and services.

1. **Test-Driven Development (TDD) Approach**
    - Applying TDD principles: write tests first, then implement the code to pass the tests.

1. **Writing Integration Tests**
    - Setting up and writing integration tests to test the entire API workflow.

1. **Introduction to Middleware in ASP.NET Core**
    - Understanding the request pipeline and creating custom middleware for handling requests.

1. **Logging with ASP.NET Core**
    - Implementing structured logging to capture important events and error information.

1. **Improving API Performance with Caching**
    - Using caching techniques to improve the performance of frequently accessed data.

1. **Rate Limiting to Protect APIs from Overuse**
    - Implementing rate limiting to control the number of requests made by clients.

1. **Using the Repository Pattern**
    - Abstracting database access using the Repository Pattern to decouple data logic from services.

1. **Implementing WebSockets for Real-Time Communication**
    - Using WebSockets to enable real-time data updates in your API.

1. **API Documentation with Swagger**
    - Integrating Swagger for automatic API documentation and exploring advanced Swagger features.

1. **Improving Security with API Keys**
    - Implementing API key-based authentication to secure access to API resources.

1. **Advanced Error Handling Techniques**
    - Handling errors with more complex strategies, including retry mechanisms and fault tolerance.
