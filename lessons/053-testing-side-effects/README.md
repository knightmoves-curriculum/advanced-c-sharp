In today's lesson we'll look at how to test side effects.  A method side effect is any observable change in state outside of the method’s return value, such as modifying a global variable, writing to a file, or logging output. Unlike a simple equation like a + b = c which can easily be tested through method parameters and the method's return type, side effects are not included in the return value.  In order to overcome this limitation, you can stub the dependency that performs this side effect.  Using a stubbed or mocked dependency allows assertion of side effects in a unit test by capturing and verifying interactions, such as ensuring a mocked logger received the expected log message.


``` cs
using MyFirstApi.Services;

namespace MyFirstApiTests;

public class DecryptionAuditServiceTest
{
    private StubLogger<DecryptionAuditService> stubLogger;
    private DecryptionAuditService service;

    public DecryptionAuditServiceTest(){
        stubLogger = new StubLogger<DecryptionAuditService>();
        service = new DecryptionAuditService(stubLogger);
    }

    [Fact]
    public void ShouldLog_WhenValueDecrypted()
    {
        var cipherText = "testCipher";
        var plaintext = "testText";

        service.OnValueDecrypted(cipherText, plaintext);

        Assert.Single(stubLogger.LoggedInfoMessages);
        Assert.Equal($"[Audit] Decrypted: {cipherText} to {plaintext}", stubLogger.LoggedInfoMessages[0]);
    }
}
```

``` cs
using Microsoft.Extensions.Logging;

class StubLogger<T> : ILogger<T>
{
    public List<string> LoggedInfoMessages { get; } = new List<string>();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (logLevel == LogLevel.Information)
        {
            LoggedInfoMessages.Add(formatter(state, exception));
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }
}
```

`dotnet test`

In the coding exercise you will test a side effect.

## Main Points
- A method side effect is any observable change in state outside of the method’s return value.
- Using a stubbed or mocked dependency allows assertion of side effects in a unit test by capturing and verifying interactions, such as ensuring a mocked logger received the expected log message.

## Suggested Coding Exercise
- Have students test a side effect

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
- https://en.wikipedia.org/wiki/Side_effect_(computer_science)

