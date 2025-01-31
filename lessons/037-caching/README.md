In today's lesson we'll look at caching. Caching is a technique that temporarily stores frequently accessed data in a faster storage layer (such as memory) in order to reduce latency and improve performance. It helps minimize expensive operations like database queries or API calls by serving cached data instead of repeatedly fetching it from the original source. However, caches must be managed carefully to ensure data consistency, prevent stale data issues, and optimize resource usage. For example, if a user's profile is updated in the database but the cache still serves the old version, the user may see outdated information until the cache expires or is refreshed.

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

using Microsoft.Extensions.Caching.Memory;
using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class CurrentWeatherForecastService
    {
        private HttpClient httpClient;
        private IWriteRepository<int, WeatherForecast> repository;
        private readonly IMemoryCache cache;
        private readonly ILogger<CurrentWeatherForecastService> logger;
        private const string CacheKey = "CurrentWeatherForecast";

        public CurrentWeatherForecastService(HttpClient httpClient, 
                                                IWriteRepository<int, WeatherForecast> repository, 
                                                IMemoryCache cache, 
                                                ILogger<CurrentWeatherForecastService> logger)
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppName/1.0");
            this.httpClient = httpClient;
            this.repository = repository;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<WeatherForecast> Report()
        {
            if (cache.TryGetValue(CacheKey, out WeatherForecast cachedForecast))
            {
                logger.LogInformation("Returning weather forecast from cache.");
                return cachedForecast!;
            }
            
            logger.LogInformation("Calling https://api.weather.gov to retrieve forecast");
            var response = await httpClient.GetFromJsonAsync<WeatherApiResponse>("https://api.weather.gov/gridpoints/DMX/73,49/forecast");

            var periods = response?.Properties.Periods;
            var weatherForecasts = (from period in periods
                        where period.Name == "This Afternoon"
                        select new WeatherForecast(period.StartTime, period.Temperature, period.ShortForecast))
                        .ToList();
            repository.Save(weatherForecasts[0]);

            cache.Set(CacheKey, weatherForecasts[0], TimeSpan.FromSeconds(10));

            return weatherForecasts[0];
        }
    }
}

public class WeatherApiResponse
{
    public required Properties Properties { get; set; }
}

public class Properties
{
    public required List<Period> Periods { get; set; }
}

public class Period
{
    public required string Name { get; set; }
    public DateTime StartTime { get; set; }
    public double Temperature { get; set; }
    public required string ShortForecast { get; set; }
}
```

`dotnet run`


In the coding exercise you will use caching.

## Main Points
- Caching is a technique that temporarily stores frequently accessed data in a faster storage layer (such as memory) in order to reduce latency and improve performance. 
- Caches must be managed carefully to ensure data consistency, prevent stale data issues, and optimize resource usage.

## Suggested Coding Exercise
- Have students add caching to their API.

## Building toward CSTA Standards:
- Evaluate the tradeoffs in how data elements are organized and where data is stored (3A-DA-10) https://www.csteachers.org/page/standards
- Justify the selection of specific control structures when tradeoffs involve implementation, readability, and program performance, and explain the benefits and drawbacks of choices made (3A-AP-15) https://www.csteachers.org/page/standards
- Evaluate algorithms in terms of their efficiency, correctness, and clarity (3B-AP-11) https://www.csteachers.org/page/standards


## Resources
- https://en.wikipedia.org/wiki/Cache_(computing)