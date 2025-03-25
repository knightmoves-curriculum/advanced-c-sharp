In today's lesson we'll look at how to write different types of test cases. Up till now we have mostly written tests for the common flow through our code.  Testing the common flow is often called happy path test cases.  Happy path test cases verify that a system functions correctly when given valid inputs and following the expected workflow without errors. These tests focus on confirming that the primary use cases work as intended under ideal conditions.  In contrast, unhappy path test cases explore how the system behaves when faced with invalid inputs, unexpected actions, or failure scenarios. They help ensure the application can handle errors gracefully and maintain stability in less-than-ideal conditions.  When you write tests remember to write both happy and unhappy test cases.


``` cs
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;
using UnitTests.Helpers;

public class UserRepositoryTest : IAsyncLifetime
{
    private readonly User _testUser;
    private UserRepository repository;
    private WeatherForecastDbContext _context;

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

    public async Task InitializeAsync()
    {
        _context = new MockDb().CreateDbContext();
        repository = new UserRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task ShouldSaveUser_WhenValidUserProvided()
    {
        var saveUser = repository.Save(_testUser);

        Assert.NotNull(saveUser);
        Assert.Equal(1, saveUser.Id);
    }

    [Fact]
    public async Task ShouldNotSaveUser_WhenInvalidUserProvided()
    {
        _testUser.Username = null;
        
        var exception = Assert.Throws<DbUpdateException>(() => repository.Save(_testUser));

        Assert.Contains("Required properties '{'Username'}' are missing for the instance of entity type 'User'.", exception.Message);
    }

    [Fact]
    public async Task ShouldFindUserByUsername_WhenUserExists()
    {
        var saveUser = repository.Save(_testUser);

        var foundUser = repository.FindByUsername(_testUser.Username);

        Assert.NotNull(foundUser);
        Assert.Equal(foundUser.Id, saveUser.Id);
    }

    [Fact]
    public async Task ShouldNotFindUserByUsername_WhenUserDoesNotExists()
    {
        var saveUser = repository.Save(_testUser);

        var foundUser = repository.FindByUsername("badName");

        Assert.Null(foundUser);
    }

    [Fact]
    public async Task ShouldRemoveUserById_WhenUserExists()
    {
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
    public async Task ShouldReturnNull_WhenRemovingUserWithAnIdThatDoesNotExist()
    {
        var foundUser = repository.RemoveById(1);

        Assert.Null(foundUser);
    }

    [Fact]
    public async Task ShouldUpdateUser_WhenUserExists()
    {
        var saveUser = repository.Save(_testUser);

        saveUser.Username = "updatedTestUsername";

        repository.Update(saveUser.Id, saveUser);
        
        var foundUser = repository.FindByUsername(saveUser.Username);
        Assert.NotNull(foundUser);
        Assert.Equal(foundUser.Id, saveUser.Id);
        Assert.Equal("updatedTestUsername", foundUser.Username);
    }

    [Fact]
    public async Task ShouldNotUpdateUser_WhenUserIsNull()
    {
        Assert.Throws<NullReferenceException>(() => repository.Update(1, null));
    }

    [Fact]
    public async Task ShouldCountUser_WhenUserExists()
    {
        var saveUser = repository.Save(_testUser);

        var count = repository.Count();

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task ShouldCountUser_WhenNoUsersExists()
    {
        var count = repository.Count();

        Assert.Equal(0, count);
    }
}
```

``` cs
namespace MyFirstApi.Models
{
    using System;

    public class UserRepository : IUserRepository
    {
        private WeatherForecastDbContext context;

        public UserRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

        public User Save(User user)
        {
            context.Users.Add(user);
            
            context.SaveChanges();
            return user;
        }

        public User Update(int id, User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
            return user;
        }


        public User FindByUsername(string username)
        {
            return context.Users.FirstOrDefault(u => u.Username == username);
        }

        public User RemoveById(int id)
        {
            var user = context.Users.Find(id);

            if(user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }

            return user;
        }

        public int Count()
        {
            return context.Users.Count();
        }
    }
}
```

`dotnet test`

In the coding exercise you will write a database test.

## Main Points
- Testing the common flow is often called happy path test cases. 
- Unhappy path test cases explore how the system behaves when faced with invalid inputs, unexpected actions, or failure scenarios.


## Suggested Coding Exercise
- Have students write unhappy path database test cases.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0
- https://en.wikipedia.org/wiki/Happy_path
