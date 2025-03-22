In today's lesson we'll look at how to test asynchronous methods.  XUnit supports testing asynchronous methods by allowing test methods to return Task or Task<T>, ensuring proper async execution and awaiting of results. The [Fact] attribute works seamlessly with async methods. Assertions and mocks can be used within the async test just like synchronous tests, verifying expected outcomes without blocking the test runner.


``` cs
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Moq;
using MyFirstApi.Models;
using MyFirstApi.Services;

public class CurrentWeatherForecastServiceTests
{
    [Fact]
    public async Task ShouldReturnsWeatherForecast_WhenApiResponseIsValid()
    {
        // Arrange
        var httpClient = new HttpClient(new StubHttpMessageHandler());
        var repositoryMock = new Mock<IWriteRepository<int, WeatherForecast>>();
        var cache = new StubMemoryCache();
        var logger = new StubLogger<CurrentWeatherForecastService>();

        var service = new CurrentWeatherForecastService(httpClient, repositoryMock.Object, cache, logger);
        
        // Act
        var result = await service.Report();
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(75, result.TemperatureF);
        Assert.Equal("Sunny", result.Summary);
        
        repositoryMock.Verify(r => r.Save(result));
        Assert.Single(logger.LoggedInfoMessages);
        Assert.Contains("Calling https://api.weather.gov to retrieve forecast", logger.LoggedInfoMessages[0]);
    }
}

public class StubMemoryCache : IMemoryCache
{
    private Dictionary<object, object> _cache = new();
    public bool TryGetValue(object key, out object? value) => _cache.TryGetValue(key, out value);
    public ICacheEntry CreateEntry(object key)
    {
        var entry = new StubCacheEntry(key, _cache);
        _cache[key] = entry.Value;
        return entry;
    }
    public void Remove(object key) => _cache.Remove(key);
    public void Dispose() { }
}

public class StubCacheEntry : ICacheEntry
{
    private object _key;
    private Dictionary<object, object> _cache;
    public object Key => _key;
    public object? Value { get; set; }
    public DateTimeOffset? AbsoluteExpiration { get; set; }
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
    public IList<IChangeToken> ExpirationTokens { get; } = new List<IChangeToken>();
    public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; } = new List<PostEvictionCallbackRegistration>();
    public CacheItemPriority Priority { get; set; }
    public long? Size { get; set; }
    public StubCacheEntry(object key, Dictionary<object, object> cache) { _key = key; _cache = cache; }
    public void Dispose() => _cache.Remove(_key);
}

public class StubHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var mockApiResponse = new WeatherApiResponse
        {
            Properties = new Properties
            {
                Periods = new List<Period>
                {
                    new Period { Name = "This Afternoon", StartTime = DateTime.UtcNow, Temperature = 75, ShortForecast = "Sunny" }
                }
            }
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(mockApiResponse))
        };
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        
        return Task.FromResult(response);
    }
}
```

`dotnet test`

In the coding exercise you will test an asynchronous method.

## Main Points
- XUnit supports testing asynchronous methods by allowing test methods to return Task or Task<T>, ensuring proper async execution and awaiting of results.

## Suggested Coding Exercise
- Have students test an asynchronous method.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
