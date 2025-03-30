In today's lesson we'll look at how to write user acceptance tests. An automated user acceptance test (UAT) verifies that a system meets business requirements by testing real-world use cases. In the testing pyramid, UAT sits at the top, as it tests the entire application flow, including third-party services and databases, and is more expensive to maintain due to its comprehensive nature. Unlike mobile or web applications that we interact with as humans, REST APIs begin their user acceptance tests at the endpoint level because the "user", in this case, is another piece of software making API calls rather than a human.

run `dotnet add package Microsoft.AspNetCore.Mvc.Testing`

``` cs
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;
using MyFirstApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.AddScoped<WeatherForecastRepository>();
builder.Services.AddScoped<IReadRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddScoped<IWriteRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddScoped<IDateQueryable<WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());
builder.Services.AddScoped<IPaginatedReadRepository<int, WeatherForecast>>(provider => provider.GetRequiredService<WeatherForecastRepository>());

builder.Services.AddScoped<CityRepository>();
builder.Services.AddScoped<IReadRepository<int, City>>(provider => provider.GetRequiredService<CityRepository>());
builder.Services.AddScoped<IWriteRepository<int, City>>(provider => provider.GetRequiredService<CityRepository>());

builder.Services.AddScoped<CityWeatherForecastRepository>();
builder.Services.AddScoped<IReadRepository<int, CityWeatherForecast>>(provider => provider.GetRequiredService<CityWeatherForecastRepository>());
builder.Services.AddScoped<IWriteRepository<int, CityWeatherForecast>>(provider => provider.GetRequiredService<CityWeatherForecastRepository>());

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ValueHasher>();
builder.Services.AddSingleton<ValueEncryptor>();

builder.Services.AddTransient<CurrentWeatherForecastService>();
builder.Services.AddHttpClient<CurrentWeatherForecastService>();

builder.Services.AddTransient<CityForecastService>();

builder.Services.AddSingleton<IDateTimeWrapper, DateTimeWrapper>();
builder.Services.AddSingleton<RateLimitingService>();

builder.Services.AddSingleton<DecryptionAuditService>();
builder.Services.AddSingleton<DecryptionLoggingService>();

builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
    options.UseSqlite("Data Source=weatherForecast.db"));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddAutoMapper(typeof(WeatherForecastProfile));

builder.Configuration.AddJsonFile("secrets.json");

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API V1", Version = "v1" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "My API V2", Version = "v2" });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        Description = "Custom header for API key",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header. Example: 'Bearer {your token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header,
            },
            new List<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>() // No specific scopes
        }
    });
});

builder.Services.AddMemoryCache();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeatherForecastDbContext>();
    db.Database.Migrate();

    var encryptor = scope.ServiceProvider.GetRequiredService<ValueEncryptor>();
    var decryptionAuditService = scope.ServiceProvider.GetRequiredService<DecryptionAuditService>();
    var decryptionLoggingService = scope.ServiceProvider.GetRequiredService<DecryptionLoggingService>();

    encryptor.ValueDecrypted += decryptionAuditService.OnValueDecrypted;
    encryptor.ValueDecrypted += decryptionLoggingService.OnValueDecrypted;
}

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/swagger"), appBuilder =>
{
    appBuilder.UseMiddleware<ApiKeyMiddleware>();
    appBuilder.UseAuthentication();
    appBuilder.UseAuthorization();
});

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "My API V2");
    c.RoutePrefix = "swagger";
});

app.Run();
public partial class Program { }
```

``` cs
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.2" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.8" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.8" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\MyFirstApi\MyFirstApi.csproj" />
    <InternalsVisibleTo Include="MyFirstApi" />
  </ItemGroup>

</Project>

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

class TokenResponse
{
    public string Token { get; set; }
}
```

``` cs
using Microsoft.Extensions.Configuration;

public class ConfigHelper
{
    public static string lookupSecret(string key)
    {
        var projectDir = Path.Combine(Directory.GetCurrentDirectory(), "../../../../../src/MyFirstApi");
        var secretsFile = Path.Combine(projectDir, "secrets.json");

        var config = new ConfigurationBuilder()
            .SetBasePath(projectDir)
            .AddJsonFile(secretsFile, optional: true, reloadOnChange: true)
            .Build();

        return config[key];
    }
}
```

`dotnet test`

In the coding exercise you will write a user acceptance test.

## Main Points
- An automated user acceptance test (UAT) verifies that a system meets business requirements by testing real-world use cases. 
- In the testing pyramid, UAT sits at the top, as it tests the entire application flow, including third-party services and databases, and is more expensive to maintain due to its comprehensive nature. 


## Suggested Coding Exercise
- Have students write a user acceptance test.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
- https://en.wikipedia.org/wiki/Acceptance_testing
- https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-9.0
