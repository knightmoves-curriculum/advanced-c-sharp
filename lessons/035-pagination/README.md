In today's lesson we'll look at pagination.  Pagination is the process of dividing large sets of data into smaller, manageable chunks, or pages, to make it easier for clients to retrieve data in parts. It helps to improve performance by reducing the amount of data sent in a single response, thus preventing timeouts or excessive memory usage. For APIs, pagination solves the problem of sending large amounts of data in one request, which can overwhelm the client or server.

``` cs
namespace MyFirstApi.Pagination
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
    }
}
```

``` cs
using System.Collections.Generic;
using MyFirstApi.Pagination;

namespace MyFirstApi.Models
{
    public interface IPaginatedReadRepository<TId, T> : IReadRepository<TId, T>, IDateQueryable<T>
    {
        PaginatedResult<T> FindPaginated(int pageNumber, int pageSize);
        PaginatedResult<T> FindPaginatedByDate(DateOnly date, int pageNumber, int pageSize);
    }
}
```

``` cs
namespace MyFirstApi.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using MyFirstApi.Pagination;

    public class WeatherForecastRepository : IWriteRepository<int, WeatherForecast>, IPaginatedReadRepository<int, WeatherForecast>
    {
        private WeatherForecastDbContext context;

        public WeatherForecastRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

        public WeatherForecast Save(WeatherForecast weatherForecast)
        {

            if(weatherForecast.Alert != null)
            {
                var alert = weatherForecast.Alert;
                alert.WeatherForecast = weatherForecast;
                context.WeatherAlerts.Add(alert);
            }
            context.WeatherForecasts.Add(weatherForecast);
            
            context.SaveChanges();
            return weatherForecast;
        }

        public WeatherForecast Update(int id, WeatherForecast weatherForecast)
        {
            weatherForecast.Id = id;
            context.WeatherForecasts.Update(weatherForecast);
            context.SaveChanges();
            return weatherForecast;
        }

        public List<WeatherForecast> FindAll()
        {
            return context.WeatherForecasts
            .Include(f => f.Alert)
            .Include(f => f.Comments)
            .Include(f => f.CityWeatherForecasts)
            .ToList();
        }

        public WeatherForecast FindById(int id)
        {
            return context.WeatherForecasts.Find(id);
        }

        public WeatherForecast RemoveById(int id)
        {
            var weatherForecast = context.WeatherForecasts.Find(id);
            context.WeatherForecasts.Remove(weatherForecast);
            context.SaveChanges();
            return weatherForecast;
        }

        public int Count()
        {
            return context.WeatherForecasts.Count();
        }

        public List<WeatherForecast> FindByDate(DateOnly date)
        {
            return context.WeatherForecasts
            .Where(wf => wf.Date == date)
            .Include(f => f.Alert)
            .Include(f => f.Comments)
            .Include(f => f.CityWeatherForecasts)
            .ToList();
        }

        public PaginatedResult<WeatherForecast> FindPaginated(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var totalCount = context.WeatherForecasts.Count();
            var items = context.WeatherForecasts
                .Include(f => f.Alert)
                .Include(f => f.Comments)
                .Include(f => f.CityWeatherForecasts)
                .OrderBy(f => f.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<WeatherForecast>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = pageNumber * pageSize < totalCount
            };
        }

        public PaginatedResult<WeatherForecast> FindPaginatedByDate(DateOnly date, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var totalCount = context.WeatherForecasts.Count();
            var items = context.WeatherForecasts
                .Where(wf => wf.Date == date)
                .Include(f => f.Alert)
                .Include(f => f.Comments)
                .Include(f => f.CityWeatherForecasts)
                .OrderBy(f => f.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<WeatherForecast>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = pageNumber * pageSize < totalCount
            };
        }
    }
}
```

In Entity Framework (EF), Skip is used to bypass a specified number of records in a query, while Take limits the number of records returned. Together, they allow for pagination by skipping over a set of records and returning a specific subset of data from a larger dataset.

``` cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;
using MyFirstApi.Pagination;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IPaginatedReadRepository<int, WeatherForecast> repository;

        public WeatherForecastController(IPaginatedReadRepository<int, WeatherForecast> repository)
        {
            this.repository = repository;
        }

        [Authorize(Policy = "UserOnly")]
        [HttpGet]
        public IActionResult Get([FromQuery] DateOnly? date, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            PaginatedResult<WeatherForecast> paginatedResult;

            if(date != null)
            {
                paginatedResult = repository.FindPaginatedByDate((DateOnly)date, pageNumber, pageSize);
            } else {
                paginatedResult = repository.FindPaginated(pageNumber, pageSize); 
            }

            var totalPages = (int)Math.Ceiling((double) paginatedResult.TotalCount / pageSize);

            var nextPageUrl = pageNumber < totalPages
                    ? Url.Action(nameof(Get), new { pageNumber = pageNumber + 1, pageSize })
                    : null;

            return Ok(new
                {
                    Forecasts = paginatedResult.Items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = paginatedResult.TotalCount,
                    TotalPages = paginatedResult.TotalPages,
                    NextPage = nextPageUrl
                });
        }

        [HttpGet("{id}")]
        public IActionResult FindById(int id)
        {
            if (id > repository.FindAll().Count)
            {
                return NotFound();
            }
            var weatherForecast = repository.FindById(id);
            return Ok(weatherForecast);
        }

        [HttpGet("boom")]
        public IActionResult Boom()
        {
            throw new InvalidOperationException("boom!");
        }
    }
}
```

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

`dotnet run`

In the coding exercise you will add pagination to your API.

## Main Points
- Pagination is the process of dividing large sets of data into smaller, manageable chunks, or pages, to make it easier for clients to retrieve data in parts.
- Pagination helps to improve performance by reducing the amount of data sent in a single response, thus preventing timeouts or excessive memory usage.
- In Entity Framework (EF), Skip is used to bypass a specified number of records in a query, while Take limits the number of records returned.

## Suggested Coding Exercise
- Have students add pagination to their API.

## Building toward CSTA Standards:
- Analyze a large-scale computational problem and identify generalizable patterns that can be applied to a solution (3B-AP-15) https://www.csteachers.org/page/standards
- Evaluate the tradeoffs in how data elements are organized and where data is stored (3A-DA-10) https://www.csteachers.org/page/standards
- Describe the issues that impact network functionality such as bandwidth, load, delay, topology (3B-NI-03) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Pagination
- https://learn.microsoft.com/en-us/ef/core/querying/pagination
