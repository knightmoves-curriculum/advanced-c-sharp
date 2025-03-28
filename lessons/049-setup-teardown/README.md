In today's lesson we'll look at setting up and tearing down tests.  Unit tests often duplicate set up steps within each test method.  
The best practice is to extract those common set up steps into a setup method.  Within XUnit this setup method is the constructor.  
In some cases tests require tear down steps after each test.  Within XUnit this is accomplished by implementing the IDisposable interface and adding teardown steps to the Dispose method.

``` cs
using System.Security.Cryptography;
using System.Text;

namespace MyFirstApiTests;

public class ValueHasherTest: IDisposable
{
    private ValueHasher? valueHasher;
    private string? testPassword;
    private string? expectedHashedPassword;

    public ValueHasherTest(){
        valueHasher = new();
        testPassword = "test";

        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(testPassword));
            expectedHashedPassword = Convert.ToBase64String(hashBytes);
        }
    }

    public void Dispose()
    {
        valueHasher = null;
        testPassword = null;
        expectedHashedPassword = null;
    }

    [Fact]
    public void ShouldHashPassword_WhenGivenValidPassword()
    {
        var hashedPassword = valueHasher.HashPassword(testPassword);

        Assert.Equal(expectedHashedPassword, hashedPassword);
    }

    [Fact]
    public void ShouldVerifyPasswordAsTrue_WhenHashPasswordAndPasswordMatch()
    {
        var isValid = valueHasher.VerifyPassword(expectedHashedPassword, testPassword);

        Assert.True(isValid);
    }

    [Fact]
    public void ShouldVerifyPasswordAsFalse_WhenHashPasswordAndPasswordDoNotMatch()
    {
        var isValid = valueHasher.VerifyPassword("bad", testPassword);

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