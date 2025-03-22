In today's lesson we'll look at how to test asynchronous methods.  XUnit supports testing asynchronous methods by allowing test methods to return Task ensuring proper async execution and awaiting of results. The [Fact] attribute works seamlessly with async methods. Assertions and mocks can be used within the async test just like synchronous tests, verifying expected outcomes without blocking the test runner.


``` cs
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public class RateLimitingMiddlewareTest
{
    private readonly StubLogger<RateLimitingMiddleware> _stubLogger;
    private readonly DefaultHttpContext _expectedHttpContext;
    private readonly StubRateLimitingService _underRateLimitService;
    private readonly StubRateLimitingService _overRateLimitService;
    private HttpContext? _actualContext = null;
    private RequestDelegate _stubRequestDelegate;
    
    // This constructor will be called before each test
    public RateLimitingMiddlewareTest()
    {
        _stubLogger = new StubLogger<RateLimitingMiddleware>();
        _expectedHttpContext = new DefaultHttpContext();
        _expectedHttpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        _expectedHttpContext.Response.Body = new MemoryStream();

        _underRateLimitService = new StubRateLimitingService(true);
        _overRateLimitService = new StubRateLimitingService(false);
        
        _stubRequestDelegate = async (HttpContext context) => _actualContext = context;
    }

    [Fact]
    public async Task ShouldCallNextMiddleware_WhenRateLimitNotExceeded()
    {
        // Arrange
        var middleware = new RateLimitingMiddleware(_stubRequestDelegate, _underRateLimitService, _stubLogger);

        // Act
        await middleware.InvokeAsync(_expectedHttpContext);

        // Assert
        _expectedHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_expectedHttpContext.Response.Body, Encoding.UTF8).ReadToEndAsync();
        Assert.Equal("", responseBody);

        Assert.Same(_expectedHttpContext, _actualContext);

        Assert.Equal(StatusCodes.Status200OK, _expectedHttpContext.Response.StatusCode);

        Assert.Equal(2, _stubLogger.LoggedDebugMessages.Count);
        Assert.Equal("Starting middleware", _stubLogger.LoggedDebugMessages[0]);
        Assert.Equal("Calling next middleware", _stubLogger.LoggedDebugMessages[1]);
    }

    [Fact]
    public async Task ShouldReturn429_WhenRateLimitExceeded()
    {
        // Arrange
        var middleware = new RateLimitingMiddleware(_stubRequestDelegate, _overRateLimitService, _stubLogger);

        // Act
        await middleware.InvokeAsync(_expectedHttpContext);

        // Assert
        _expectedHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_expectedHttpContext.Response.Body, Encoding.UTF8).ReadToEndAsync();
        Assert.Equal("Rate limit exceeded. Try again later.", responseBody);

        Assert.Null(_actualContext);

        Assert.Equal(StatusCodes.Status429TooManyRequests, _expectedHttpContext.Response.StatusCode);
        
        Assert.Single(_stubLogger.LoggedDebugMessages);
        Assert.Equal("Starting middleware", _stubLogger.LoggedDebugMessages[0]);
    }
}

public class StubRateLimitingService : RateLimitingService
{
    private readonly bool _isRequestAllowed;

    public StubRateLimitingService(bool isRequestAllowed) 
        : base(new DateTimeWrapper()) // Always passes a new DateTimeWrapper
    {
        _isRequestAllowed = isRequestAllowed;
    }

    public override bool IsRequestAllowed(string clientKey)
    {
        return _isRequestAllowed; // Always returns the predefined value
    }
}
```

``` cs
using Microsoft.Extensions.Logging;

class StubLogger<T> : ILogger<T>
{
    public List<string> LoggedInfoMessages { get; } = new List<string>();
    public List<string> LoggedDebugMessages { get; } = new List<string>();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (logLevel == LogLevel.Information)
        {
            LoggedInfoMessages.Add(formatter(state, exception));
        }
        if (logLevel == LogLevel.Debug)
        {
            LoggedDebugMessages.Add(formatter(state, exception));
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

In the coding exercise you will test an asynchronous method.

## Main Points
- XUnit supports testing asynchronous methods by allowing test methods to return Task ensuring proper async execution and awaiting of results.

## Suggested Coding Exercise
- Have students test an asynchronous method.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
