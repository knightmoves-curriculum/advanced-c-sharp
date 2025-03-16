In today's lesson we'll look at how to test exceptions.  XUnit test cases fail when the code they are testing throws an exception because, 
by default, a test is expected to complete successfully without unexpected errors.
The Assert.Throws method in XUnit is used to verify that a specific exception is thrown by a given piece of code, ensuring that expected failures 
are correctly handled in tests. By using Assert.Throws, a test can pass if the expected exception occurs, allowing for controlled validation of 
error-handling behavior.

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
    public void ShouldThrowException_WhenGivenNullPassword()
    {
        try{
            valueHasher.HashPassword(null);
            Assert.Fail();
        } 
        catch (ArgumentNullException e)
        {
            Assert.NotNull(e);
        }

        Assert.Throws<ArgumentNullException>(() => {valueHasher.HashPassword(null)});
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

In the coding exercise you will use Assert.Throws<>.

## Main Points
- XUnit test cases fail when the code they are testing throws an exception because, by default, a test is expected to complete successfully without unexpected errors.
- The Assert.Throws<> method in XUnit is used to verify that a specific exception is thrown by a given piece of code, ensuring that expected failures are correctly handled in tests.

## Suggested Coding Exercise
- Have students test code that throws an error.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/

