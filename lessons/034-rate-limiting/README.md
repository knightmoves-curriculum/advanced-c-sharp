In today's lesson we'll look at rate limiting.  Rate limiting in an API is a technique used to control the number of requests a client can make to the server within a specific time period, ensuring fair usage and protecting against abuse like DDoS attacks. A DDoS (Distributed Denial of Service) attack is an attempt to overwhelm a server with an enormous number of requests from multiple sources, causing it to slow down or crash. Rate limiting helps maintain server performance, prevent resource overloading, and enforce usage policies for API consumers.

``` cs
public class RateLimitingService
{
    private readonly Dictionary<string, List<DateTime>> _requests = new();
    private readonly int _maxRequests = 10;
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1); 

    public bool IsRequestAllowed(string clientKey)
    {
        if (!_requests.ContainsKey(clientKey))
        {
            _requests[clientKey] = new List<DateTime>();
        }

        _requests[clientKey].RemoveAll(request => request < DateTime.UtcNow - _timeWindow);

        if (_requests[clientKey].Count >= _maxRequests)
        {
            return false;
        }

        _requests[clientKey].Add(DateTime.UtcNow);
        return true;
    }
}

```

``` cs
using Microsoft.AspNetCore.Localization;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimitingService _rateLimitingService;
    private readonly ILogger<RateLimitingMiddleware> logger;


    public RateLimitingMiddleware(RequestDelegate next, RateLimitingService rateLimitingService, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _rateLimitingService = rateLimitingService;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogDebug("Starting middleware");
        var clientKey = context.Connection.RemoteIpAddress.ToString();

        if (!_rateLimitingService.IsRequestAllowed(clientKey))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            return;
        }
        logger.LogDebug("Calling next middleware");
        await _next(context);
    }
}
```

As we defined before, middleware in ASP.NET Core is software injected into the request pipeline to handle HTTP requests and responses, allowing for tasks such as authentication, logging, or response modification. The request pipeline is a sequence of middleware components that process incoming HTTP requests and outgoing responses, determining how the application handles and responds to each request.

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


builder.Services.AddSingleton<RateLimitingService>();

builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
    options.UseSqlite("Data Source=weatherForecast.db"));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddAutoMapper(typeof(WeatherForecastProfile));

builder.Configuration.AddJsonFile("secrets.json");
        
builder.Services.AddAuthentication("JwtAuthentication")
    .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>("JwtAuthentication", options => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeatherForecastDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<RateLimitingMiddleware>();

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
```

``` cs
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class ApiKeyMiddleware
{
    private const string APIKEYNAME = "X-Api-Key";
    private readonly RequestDelegate _next;
    private string apiKey;
    private readonly ILogger<ApiKeyMiddleware> logger;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        apiKey = configuration["ApiKey"];
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogDebug("Starting middleware");
        if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key was not provided.");
            return;
        }

        if (!apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }
        logger.LogDebug("Calling next middleware");
        await _next(context);
    }
}
```

``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Error",
      "JwtAuthenticationHandler": "Error"
    }
  }
}

```

`dotnet run`

In the coding exercise you will add rate limiting to your API.

## Main Points
- Rate limiting in an API is a technique used to control the number of requests a client can make to the server within a specific time period, ensuring fair usage and protecting against abuse
- A DDoS (Distributed Denial of Service) attack is an attempt to overwhelm a server with an enormous number of requests from multiple sources, causing it to slow down or crash.
- The request pipeline is a sequence of middleware components that process incoming HTTP requests and outgoing responses, determining how the application handles and responds to each request.

## Suggested Coding Exercise
- Have students add rate limiting to their API.

## Building toward CSTA Standards:
- Describe the issues that impact network functionality (e.g., bandwidth, load, delay, topology) (3B-NI-03) https://www.csteachers.org/page/standards
- Compare ways software developers protect devices and information from unauthorized access (3B-NI-04) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Rate_limiting
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0