In today's lesson we'll look at unit testing.  Unit testing is the process of testing a single method in complete isolation to verify that it produces the expected output for given inputs.  Think of a method like a mathmatical equation like a + b = c.  In a method the first part of the mathmatical equation a + b come in as parameters and the result c is the return type of the method.  A unit test acts alot like a mathematical proof that ensures the expected results always holds true.  One popular .NET testing framework used in unit testing is XUnit.  XUnit is a popular open-source testing framework that provides a lightweight, extensible way to write and run automated tests.

1. Create `src` folder at the root of the project. The src folder in a .NET solution contains the main application code, including all projects, source files, configurations, and dependencies required to build and run the application.
1. Create `test` folder at the root of the project.The test folder in a .NET solution holds test projects that contain unit, integration, or other types of automated tests to verify the functionality and correctness of the application code in the src folder.
1. Create `MyFirstApi` folder inside the `src` folder.
1. Move all of the files except `.gitignore` and the `.sln` file into the `src/MyFirstApi` folder.
1. run `dotnet sln remove MyFirstApi.csproj` The command dotnet sln remove MyFirstApi.csproj removes the MyFirstApi.csproj project from the solution file (.sln), but it does not delete the project files from the filesystem.
1. run `dotnet sln add src/MyFirstApi/MyFirstApi.csproj`The command dotnet sln add src/MyFirstApi/MyFirstApi.csproj adds the MyFirstApi project to the solution file (.sln), allowing it to be managed as part of the overall .NET solution.
1. run `cd test/`
1. run `dotnet new xunit -n MyFirstApiTests` The command dotnet new xunit -n MyFirstApiTests creates a new xUnit test project named MyFirstApiTests, generating the necessary files and configuration to write and run unit tests in .NET.
1. run `cd ../../`
1. run `dotnet add test/MyFirstApiTests/MyFirstApiTests.csproj reference src/MyFirstApi/MyFirstApi.csproj` This command adds a project reference in test/MyFirstApiTests/MyFirstApiTests.csproj, allowing it to access and test the classes defined in src/MyFirstApi/MyFirstApi.csproj.
1. run `dotnet test`
1. Create a `Security` folder inside the `test/MyFirstApiTests/` folder
1. Create a new class named `ValueHasherTest.cs` inside the `test/MyFirstApiTests/` folder

``` cs
namespace MyFirstApiTests;

public class ValueHasherTest
{
    [Fact]
    public void ShouldHashPassword_WhenGivenValidPassword()
    {
        var hashedPassword = new ValueHasher().HashPassword("test");

        Assert.Equal("n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=", hashedPassword);
    }

    [Fact]
    public void ShouldVerifyPasswordAsTrue_WhenHashPasswordAndPasswordMatch()
    {
        var isValid = new ValueHasher().VerifyPassword("n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=", "test");

        Assert.True(isValid);
    }

    [Fact]
    public void ShouldVerifyPasswordAsFalse_WhenHashPasswordAndPasswordDoNotMatch()
    {
        var isValid = new ValueHasher().VerifyPassword("bad", "test");

        Assert.False(isValid);
    }
}
```

`dotnet test`

In the coding exercise you will write a unit test.

## Main Points
- Unit testing is the process of testing a single method in complete isolation to verify that it produces the expected output for given inputs.
- A unit test acts like a mathematical proof that ensures the expected results always holds true.
- XUnit is a popular open-source testing framework that provides a lightweight, extensible way to write and run automated tests.

## Suggested Coding Exercise
- Have students unit test a method that has no external dependencies like a utility method.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
