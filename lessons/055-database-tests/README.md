In today's lesson we'll look at how to write database tests.  Up until now we have been focused on writing unit tests that test a method in complete isolation.  In order to write a database test a real db context has to be created instead of mocking out this dependency.  If you think about testing like a pyramid, the wide base is composed of unit tests and cover every part of the application.  As you move up beyond these foundational unit tests these tests begin to integrate multiple real dependencies together.  At the very top of the pyramid the entire application is loaded with 100% of the dependencies wired together.  As you move from the bottom of this testing pyramid to the top, the execution time of your tests typically gets slower and slower and the cost of writing and maintaining them typically gets more and more expensive.  In order to provide maximum coverage while maximizing the speed and cost of this suite of tests, it's a best practice to only write tens of full-stack integration tests at the top of the pyramid, hundreds of integration tests in the middle of the pyramid and thousands of unit tests at the bottom of the pyramid. For this lesson we write our first integration test in the middle of the testing pyramid. 


``` cs
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;

public class MockDb : IDbContextFactory<WeatherForecastDbContext>
{
    public WeatherForecastDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<WeatherForecastDbContext>()
            .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

        return new WeatherForecastDbContext(options);
    }
}
```

``` cs
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;

public class UserRepositoryTest
{
    private readonly User _testUser;

    public UserRepositoryTest()
    {
        _testUser = new User
        {
            Id = 0,
            Username = "testUsername",
            Role = "testRole",
            HashedPassword = "testHashedPassword",
            EncryptedSocialSecurityNumber = "testEncryptedSocialSecurityNumber"
        };
    }

    [Fact]
    public async Task ShouldSaveUser_WhenValidUserProvided()
    {
        await using var context = new MockDb().CreateDbContext();
        var repository = new UserRepository(context);
        
        var saveUser = repository.Save(_testUser);

        Assert.NotNull(saveUser);
        Assert.Equal(1, saveUser.Id);
    }

    [Fact]
    public async Task ShouldFindUserByUsername_WhenUserExists()
    {
        await using var context = new MockDb().CreateDbContext();
        var repository = new UserRepository(context);
        var saveUser = repository.Save(_testUser);

        var foundUser = repository.FindByUsername(_testUser.Username);

        Assert.NotNull(foundUser);
        Assert.Equal(foundUser.Id, saveUser.Id);
    }

    [Fact]
    public async Task ShouldRemoveUserById_WhenUserExists()
    {
        await using var context = new MockDb().CreateDbContext();
        var repository = new UserRepository(context);
        var saveUser = repository.Save(_testUser);
        var countBefore = repository.Count();

        var foundUser = repository.RemoveById(saveUser.Id);

        var countAfter = repository.Count();
        Assert.NotNull(foundUser);
        Assert.Equal(foundUser.Id, saveUser.Id);
        Assert.Equal(1, countBefore);
        Assert.Equal(0, countAfter);
    }

    [Fact]
    public async Task ShouldUpdateUser_WhenUserExists()
    {
        await using var context = new MockDb().CreateDbContext();
        var repository = new UserRepository(context);
        var saveUser = repository.Save(_testUser);

        saveUser.Username = "updatedTestUsername";

        repository.Update(saveUser.Id, saveUser);
        
        var foundUser = repository.FindByUsername(_testUser.Username);
        Assert.NotNull(foundUser);
        Assert.Equal(foundUser.Id, saveUser.Id);
        Assert.Equal("updatedTestUsername", foundUser.Username);
    }

    [Fact]
    public async Task ShouldCountUser_WhenUserExists()
    {
        await using var context = new MockDb().CreateDbContext();
        var repository = new UserRepository(context);
        var saveUser = repository.Save(_testUser);

        var count = repository.Count();

        Assert.Equal(1, count);
    }
}
```

`dotnet test`

In the coding exercise you will write a database test.

## Main Points
- If you think about testing like a pyramid, the base is composed of unit tests and cover every part of the application.  As you move up beyond these foundational unit tests these tests integrate multiple dependencies together.  At the very top of the pyramid the entire application is loaded with 100% of the dependencies wired together.  
- As you move from the bottom of this testing pyramid to the top the execution time of your tests typically gets slower and slower and the cost of writing and maintaining them typically gets more expensive.  
- In order to provide maximum coverage while maximizing the speed and cost of this suite of tests, it is a best practice to write tens of full-stack integration tests at the top of the pyramid, hundreds of integration tests in the middle of the pyramid and thousands of unit tests at the bottom of the pyramid.  


## Suggested Coding Exercise
- Have students write a database test.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0
