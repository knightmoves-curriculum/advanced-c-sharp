In today's lesson we'll look at code coverage. Code coverage measures the percentage of your codebase that is executed by automated tests, helping to identify untested parts of your application. High coverage ensures critical paths are tested, reducing the risk of bugs and improving software reliability. While 100% coverage doesn’t guarantee bug-free code, it provides insight into test effectiveness and encourages better testing practices.

run `cd test/MyFirstApiTests`

run `dotnet add package coverlet.msbuild`

run `cd ../../`

run `dotnet test -p:CollectCoverage=true -p:CoverletOutputFormat=lcov -p:CoverletOutput=coverage.info`

run `reportgenerator -reports:test/MyFirstApiTests/coverage.info -targetdir:CoverageReport -reporttypes:Html`


```
**/bin/
**/obj/
.vscode/
*.secrets.json
secrets.json
/CoverageReport
**/**/coverage.info
```

``` cs
using System.Net;
using System.Net.Http.Json;

public class JwtHelper{
    public static  async Task<string> GenerateTokenAsync(HttpClient client, string role)
    {
        var guid = Guid.NewGuid().ToString();
        var userDto = new UserDtoV1
        {
                Username = "TestUser" + guid,
                Password = "TestPassword" + guid,
                Role = role,
                SocialSecurityNumber = "123-45-6789"
        };

        var initialSetupResponse = await client.PostAsJsonAsync("/v1/authentication/register", userDto);
        Assert.Equal(HttpStatusCode.OK, initialSetupResponse.StatusCode);

        var response = await client.PostAsJsonAsync("/v1/authentication/token", userDto);
        var content = await response.Content.ReadFromJsonAsync<TokenResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return content?.Token;
    }
}

class TokenResponse
{
    public string Token { get; set; }
}
```

``` cs
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

public class UserAcceptanceTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    protected readonly HttpClient _client;
    private readonly string _role;

    public UserAcceptanceTest(WebApplicationFactory<Program> factory, string role)
    {
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Add("X-API-Key", ConfigHelper.lookupSecret("ApiKey"));

        _role = role;
    }

    public async Task InitializeAsync()
    {
        var token = await JwtHelper.GenerateTokenAsync(_client, _role);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
```

``` cs
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
public class AuthenticationV1ControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly UserDtoV1 _userDto;
    
    public AuthenticationV1ControllerTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        var guid = Guid.NewGuid().ToString();
        _userDto = new UserDtoV1
        {
                Username = "TestUser" + guid,
                Password = "TestPassword" + guid,
                Role = "Admin",
                SocialSecurityNumber = "123-45-6789"
        };

        _client.DefaultRequestHeaders.Add("X-API-Key", ConfigHelper.lookupSecret("ApiKey"));
    }

    [Fact]
    public async Task ShouldCreateUser_WhenValidRequest()
    {
        var response = await _client.PostAsJsonAsync("/v1/authentication/register", _userDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("User registered successfully.", content);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenUsernameAlreadyExists()
    {
        var initialSetupResponse = await _client.PostAsJsonAsync("/v1/authentication/register", _userDto);
        Assert.Equal(HttpStatusCode.OK, initialSetupResponse.StatusCode);

        var response = await _client.PostAsJsonAsync("/v1/authentication/register", _userDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Username is already taken.", content);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenValidCredentialsProvided()
    {
        var initialSetupResponse = await _client.PostAsJsonAsync("/v1/authentication/register", _userDto);
        Assert.Equal(HttpStatusCode.OK, initialSetupResponse.StatusCode);

        var response = await _client.PostAsJsonAsync("/v1/authentication/token", _userDto);
        var content = await response.Content.ReadFromJsonAsync<TokenResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content.Token);
        Assert.Equal(396, content.Token.Length);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenInvalidCredentialsProvided()
    {
        var response = await _client.PostAsJsonAsync("/v1/authentication/token", _userDto);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Invalid username or password.", content);
    }
}
```

``` cs
using System.Net;
using MyFirstApi.Pagination;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http.Headers;
public class WeatherForecastAdminControllerTest : UserAcceptanceTest
{
    private readonly WeatherForecastDto _weatherForecastDto;

    public WeatherForecastAdminControllerTest(WebApplicationFactory<Program> factory): base(factory, "Admin")
    {
        var result = new PaginatedResult<WeatherForecast>();
        result.Items = new List<WeatherForecast>();
        _weatherForecastDto = new WeatherForecastDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            TemperatureF = 49,
            Summary = "test summary",
            Alert = "test alert",
            Comments = new List<string> { "test comment" },
            CityIds = new List<int>()
        };
    }

    [Fact]
    public async Task ShouldCreateWeatherForecast_WhenGivenValidWeatherForecast()
    {
        var response = await _client.PostAsJsonAsync("/admin/WeatherForecast", _weatherForecastDto);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdWeatherForecast = await response.Content.ReadFromJsonAsync<WeatherForecast>();
        Assert.NotNull(createdWeatherForecast);
        Assert.True(createdWeatherForecast.Id > 0);
        Assert.Equal(_weatherForecastDto.TemperatureF, createdWeatherForecast.TemperatureF);
        Assert.Equal(_weatherForecastDto.Summary, createdWeatherForecast.Summary);
        Assert.Equal(_weatherForecastDto.Alert, createdWeatherForecast.Alert.AlertMessage);
        Assert.Equal(_weatherForecastDto.Comments, createdWeatherForecast.Comments?.Select(c => c.CommentMessage).ToList());
    }

    [Fact]
    public async Task ShouldNotAllow_WhenUserIsNotAdmin()
    {
        var token = await JwtHelper.GenerateTokenAsync(_client, "NotAdmin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync("/admin/WeatherForecast", _weatherForecastDto);
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ShouldNotCreateWeatherForecast_WhenTemperatureOver50Degrees()
    {
        _weatherForecastDto.TemperatureF = 51;

        var response = await _client.PostAsJsonAsync("/admin/WeatherForecast", _weatherForecastDto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Temperature must be between -30 and 50 degrees.", content);
    }

    [Fact]
    public async Task ShouldNotCreateWeatherForecast_WhenTemperatureLessThanNegative30Degrees()
    {
        _weatherForecastDto.TemperatureF = -31;

        var response = await _client.PostAsJsonAsync("/admin/WeatherForecast", _weatherForecastDto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Temperature must be between -30 and 50 degrees.", content);
    }
}
```

`dotnet test`

In the coding exercise you will generate code coverage.

## Main Points
- Code coverage measures the percentage of your codebase that is executed by automated tests, helping to identify untested parts of your application. 


## Suggested Coding Exercise
- Have students generate their code coverage

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
- https://github.com/coverlet-coverage/coverlet
