In today's lesson we'll look at test stubs.  Stubbing within the context of unit testing refers to replacing a real method or function with a simplified version that returns predefined values, allowing control over the behavior of dependencies. This technique isolates the unit under test by preventing real interactions with external components, such as databases or APIs. It makes dependencies controllable and predictable by defining their responses, ensuring consistent and reliable test results.

``` cs

namespace MyFirstApiTests;

public class RateLimitingServiceTest
{
    private DateTime currentDateTime;
    private RateLimitingService rateLimitingService;
    private StubDateTimeWrapper stubDateTimeWrapper;

    public RateLimitingServiceTest(){
        currentDateTime = DateTime.UtcNow;

        stubDateTimeWrapper = new StubDateTimeWrapper(currentDateTime);

        rateLimitingService = new(stubDateTimeWrapper);
    }

    [Fact]
    public void ShouldAllowRequest_WhenTenthRequestIsReceivedWithinOneMinute(){
        //Arrange
        for (int i = 0; i < 9; i++)
        {
                rateLimitingService.IsRequestAllowed("test");
        }

        //Act
        var isAllowed = rateLimitingService.IsRequestAllowed("test");

        //Assert
        Assert.True(isAllowed);
    }

    [Fact]
    public void ShouldNotAllowRequest_WhenEleventhRequestIsReceivedWithinOneMinute(){
        //Arrange
        for (int i = 0; i < 10; i++)
        {
                rateLimitingService.IsRequestAllowed("test");
        }

        //Act
        var isAllowed = rateLimitingService.IsRequestAllowed("test");

        //Assert
        Assert.False(isAllowed);
    }

    [Fact]
    public void ShouldAllowRequest_WhenEleventhRequestIsReceivedOverOneMinuteLater(){
        //Arrange
        var initialTime = DateTime.Parse("2000-01-01 01:01:01");
        stubDateTimeWrapper.SetUp(initialTime);

        for (int i = 0; i < 10; i++)
        {
                rateLimitingService.IsRequestAllowed("test");
        }

        var oneMinuteOneMillisecondLater = initialTime.AddMinutes(1).AddMilliseconds(1);
        stubDateTimeWrapper.SetUp(oneMinuteOneMillisecondLater);

        //Act
        var isAllowed = rateLimitingService.IsRequestAllowed("test");

        //Assert
        Assert.True(isAllowed);
    }
}

class StubDateTimeWrapper : IDateTimeWrapper
{
    private DateTime dateTime;

    public StubDateTimeWrapper(DateTime dateTime)
    {
        this.dateTime = dateTime;
    }

    public void SetUp(DateTime dateTime)
    {
        this.dateTime = dateTime;
    }
    public DateTime UtcNow()
    {
        return dateTime;
    }
}
```

``` cs
public class RateLimitingService
{
    private readonly Dictionary<string, List<DateTime>> _requests = new();
    private readonly int _maxRequests = 10;
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);
    private IDateTimeWrapper dateTimeWrapper;

    public RateLimitingService(IDateTimeWrapper dateTimeWrapper)
    {
        this.dateTimeWrapper = dateTimeWrapper;
    }

    public bool IsRequestAllowed(string clientKey)
    {
        if (!_requests.ContainsKey(clientKey))
        {
            _requests[clientKey] = new List<DateTime>();
        }

        _requests[clientKey].RemoveAll(request => request < dateTimeWrapper.UtcNow() - _timeWindow);

        if (_requests[clientKey].Count >= _maxRequests)
        {
            return false;
        }

        _requests[clientKey].Add(dateTimeWrapper.UtcNow());
        return true;
    }
}

```

``` cs
public interface IDateTimeWrapper
{
    DateTime UtcNow();
}
```

``` cs
public class DateTimeWrapper: IDateTimeWrapper
{
    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}
```

``` cs
builder.Services.AddSingleton<IDateTimeWrapper, DateTimeWrapper>();
```

`dotnet test`

In the coding exercise you will stub a test dependency.

## Main Points
- Stubbing within the context of unit testing refers to replacing a real method or function with a simplified version that returns predefined values, allowing control over the behavior of dependencies.

## Suggested Coding Exercise
- Have students stub a test dependency.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
- https://en.wikipedia.org/wiki/Test_stub

