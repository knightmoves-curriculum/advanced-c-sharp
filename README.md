# Advanced OOP - ASP.NET Core REST API Course Topics

1. **First API**
   - Install .NET SDK and create a basic Web API project using the .NET CLI.
   - Overview of the project structure

1. **Creating a GET Endpoint**
   - Adding and customizing a basic GET endpoint.
   - Understanding routing conventions and customizing routes for API endpoints.
  
1. **Creating a POST Endpoint**
   - Adding functionality to handle POST requests and create new resources.

1. **Working with Route Parameters**
   - Using route parameters for fetching resources dynamically based on URL values.

1. **Implementing a PUT Endpoint**
   - Handling updates to existing resources using the PUT method.
     
1. **Implementing a DELETE Endpoint**
   - Removing resources with the DELETE method and handling responses.

1. **2XX Responses**

1. **Model Validation with Data Annotations**
    - Using data annotations to validate input models and ensure data integrity.

1. **4XX Responses**
     - 400 Bad Request when model fails validation

1. **Custom Responses**

1. **Single Responsibility Principle (SRP)**
   - Extract new store class and new it up in the controller
     
1. **Dependency Inversion Principle (DIP)**
    - Refactoring business logic into services and injecting them into controllers.
    - Factory pattern
    - Exploring how DI works in ASP.NET Core and setting up services.
    - Inject using concrete classes

1. **Service Lifetimes: Scoped, Transient, and Singleton**
    - Understanding different service lifetimes and when to use each one.

1. **Interface Segregation Principle (ISP)**
   - Inject using interfaces

1. **Working with Task-Based Asynchronous Patterns (TAP)**
    - Deep dive into TAP and its use in improving concurrency in APIs.
   - Introduce asynchronous programming using `async` and `await` to write non-blocking code.

1. **Introduction to Entity Framework Core and SQLite Setup**
    - Setting up Entity Framework Core with SQLite as a lightweight local database.

1. **One-to-one Relationships**
    - Modeling data with entity relationships

1. **One-to-many Relationships**
    - Modeling data with entity relationships
  
1. **Using DTOs (Data Transfer Objects) in APIs**
    - Separating internal models from external-facing DTOs for better API design.

1. **Mapping Entities to DTOs with AutoMapper**
    - Using AutoMapper to simplify mapping between domain models and DTOs.
   
1. **Many-to-many Relationships**
    - Modeling data with entity relationships

1. **Handling Query Parameters in API Requests**
   - Implementing dynamic filtering or searching using query parameters.
   - Demonstrate basic LINQ queries on collections, such as filtering, projecting, and ordering data.
   - Use LINQ methods like `Aggregate`, `Sum`, `Count`, and `Average` to perform aggregation and reduction on collections.

1. **Advanced Validation Techniques**
    - Creating custom validation attributes and business rule validation.

1. **Global Error Handling and Exception Filters**
    - Implementing centralized error handling and creating custom exception filters.

1. **Implementing Basic Authentication**
    - Securing endpoints using basic authentication.
    - Basic principles of securing an API, including authentication and authorization.

1. **Securing APIs with JWT (JSON Web Tokens)**
    - Implementing JWT authentication to secure API endpoints in a stateless manner.

1. **Role-Based Authorization**
    - Restricting access to API endpoints based on user roles.

1. **One Way Hashing**
    - Techniques for securely handling and storing sensitive information.
  
1. **Encryption**
    - Techniques for securely handling and storing sensitive information.

1. **Improving Security with API Keys**
    - Implementing API key-based authentication to secure access to API resources.

1. **Versioning an API**
    - Strategies for implementing version control in a REST API to support backward compatibility.
    - Introduce Middleware 

1. **API Documentation with Swagger**
    - Integrating Swagger for automatic API documentation and exploring advanced Swagger features.

1. **Logging with ASP.NET Core**
    - Implementing structured logging to capture important events and error information.

1. **Rate Limiting to Protect APIs from Overuse**
    - Implementing rate limiting to control the number of requests made by clients.
    - Understanding the request pipeline and creating custom middleware for handling requests.

1. **Pagination and Sorting**
    - Implementing pagination and sorting mechanisms for handling large data sets.

1. **Understanding Delegates and Events**
    - Implementing delegates and events to decouple components in services.

1. **Improving API Performance with Caching**
    - Using caching techniques to improve the performance of frequently accessed data.

1. **Parallel Programming with PLINQ**
   - Demonstrate how to execute LINQ queries in parallel using Parallel LINQ (PLINQ) for performance gains in CPU-bound tasks.

1. **Func, Action, and Predicate**
   - Learn how to use the `Func<T>`, `Action<T>`, and `Predicate<T>` delegate types to create reusable function pointers.

1. **Nullable Types and the Nullable<T> Structure**
   - Explain the use of nullable value types (`int?`, `bool?`) and how the `Nullable<T>` struct works in C#.
   - Enforcing nullability rules to improve code safety and reduce null reference exceptions.

1. **Null-Conditional Operators**
   - Demonstrate how to handle `null` safely with operators like `?.` (null-conditional).

1. **Null-Coalescing**
   - Demonstrate how to handle `null` safely with operators like `??` (null-coalescing).

1. **Interface Default Methods in C# 8**
   - Introduce default interface methods and demonstrate how to provide method implementations in interfaces.

1. **Generic Constraints**
   - Teach how to create generic classes and methods that can operate on any data type.
   - Explain how to use constraints (`where T : class`, `where T : struct`) to limit the types that can be passed to a generic class or method.

1. **Exception Handling with `try`, `catch`, and `finally`**
   - Review the basic concepts of exception handling and how to properly use `try`, `catch`, and `finally` blocks.

1. **Creating and Using Attributes in C#**
   - Show how to define and use custom attributes to annotate and provide metadata for classes and methods.

1. **`IEnumerable` vs `IQueryable`: Differences and Use Cases**
   - Discuss the differences between `IEnumerable` and `IQueryable` and when to use each in C# for data querying.

1. **Unit Testing**
1. **Setup & Teardown**
1. **Testing Exceptions**
1. **Test Stubs**
1. **Test Mocks**
1. **Testing Side Effects**
1. **Testing Asynchronous Code**
1. **Stubbing out 3rd Party Services for Testing**
1. **Testing the Database & Code Coverage**
1. **Refactor Packaging Similar Classes for Readability  & Test-Driven Development (TDD) Approach**
1. **Writing Integration Tests**  
1. **Refactor Duplicated Code**
1. **Refactor Long Method**
