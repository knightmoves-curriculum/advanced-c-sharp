In today's lesson we'll look at setting up and tearing down tests.  Unit tests often duplicate set up steps within each test method.  The best practice is to extract those common set up steps into a setup method.  Within XUnit this setup method is the constructor.  In some cases tests require tear down steps after each test.  Within XUnit this is accomplished by implementing the IDisposable interface and adding teardown steps to the Dispose method.

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
- Unit tests often duplicate set up steps within each test method.  
- The best practice is to extract those common set up steps into a setup method. 

## Suggested Coding Exercise
- Have students refactor common set up steps into the constructor.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
