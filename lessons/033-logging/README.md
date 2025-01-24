In today's lesson we'll look at logging.  Logging is the process of recording application events, errors, and important runtime information to help developers monitor and troubleshoot their application. Adding logging to your application improves visibility into its behavior, making it easier to detect issues, analyze performance, and maintain reliable operations.

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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private ILogger<GlobalExceptionFilter> logger;
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        this.logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        logger.LogError(exception, "An error occurred: {ErrorMessage}", exception.Message);

        var response = new
        {
            message = "An unexpected error occurred.",
            error = context.Exception.Message
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = 500
        };
    }
}
```

``` cs
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFirstApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/authentication")]
public class AuthenticationV1Controller : ControllerBase
{
    private readonly string issuer;
    private readonly string audience;
    private readonly string secret;
    private readonly IUserRepository userRepository;
    private readonly ValueHasher passwordHasher;
    private readonly ValueEncryptor valueEncryptor;
    private readonly IMapper mapper;
    private readonly ILogger<AuthenticationV2Controller> logger;

    public AuthenticationV1Controller(IConfiguration configuration, 
                                    IUserRepository userRepository, 
                                    ValueHasher passwordHasher, 
                                    ValueEncryptor valueEncryptor,
                                    IMapper mapper,
                                    ILogger<AuthenticationV2Controller> logger)
    {
        issuer = configuration["Jwt:Issuer"];
        audience = configuration["Jwt:Audience"];
        secret = configuration["Jwt:Secret"];
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.valueEncryptor = valueEncryptor;
        this.mapper = mapper;
        this.logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDtoV1 userDto)
    {
        logger.LogDebug("Registering user: {Username}", userDto.Username);

        var existingUser = userRepository.FindByUsername(userDto.Username);
        if (existingUser != null)
        {
            var message = "Username is already taken.";
            logger.LogInformation(message + " For " + userDto.Username + ".");

            return BadRequest("Username is already taken.");
        }

        var user = mapper.Map<User>(userDto);

        string hashPassword = passwordHasher.HashPassword(userDto.Password);
        logger.LogInformation("Hashed Password: " + hashPassword);
        user.HashedPassword = hashPassword;

        string encryptedSocialSecurityNumber = valueEncryptor.Encrypt(userDto.SocialSecurityNumber);
        logger.LogInformation("Encrypted Social Security Number: " + encryptedSocialSecurityNumber);
        user.EncryptedSocialSecurityNumber = encryptedSocialSecurityNumber;

        userRepository.Save(user);
        return Ok("User registered successfully.");
    }

    [HttpPost("token")]
    public IActionResult Token([FromBody] UserDtoV1 userDto)
    {

        logger.LogDebug("Generating a token for user: {Username}", userDto.Username);

        var user = userRepository.FindByUsername(userDto.Username);
        if (user == null || !passwordHasher.VerifyPassword(user.HashedPassword, userDto.Password))
        {
            var message = "Invalid username or password.";
            logger.LogInformation(message + " For " + userDto.Username + ".");
            return Unauthorized(message);
        }

        string socialSecurityNumber = valueEncryptor.Decrypt(user.EncryptedSocialSecurityNumber);
        logger.LogInformation("Decrypted Social Security Number: " + socialSecurityNumber);

        string token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        logger.LogTrace("Starting to generate JWT token for user: {Username}", user.Username);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);

        logger.LogTrace("Finished generating JWT token");

        return tokenHandler;
    }
}
```

``` json
// appsettings.Development.json

{
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}

```

ASP.NET provides a number of logging levels which indicate the severity or importance of logged events. In development, a lower minimum logging level like Debug or Trace is often used to capture detailed diagnostic information, aiding debugging and testing. Lower logging levels, like Debug or Trace, generate more detailed and frequent logs, which often negatively impact performance by increasing CPU usage, memory consumption, and the amount of reading and writing to the computer.  In production, a higher level like Warning or Error minimizes improves logging noise, focusing on critical issues while improving performance and conserving resources.

`dotnet run`

In the coding exercise you will add logging to your application.

## Main Points
- Logging is the process of recording application events, errors, and important runtime information to help developers monitor and troubleshoot their application.
- ASP.NET provides a number of logging levels which indicate the severity or importance of logged events.
- Lower logging levels, like Debug or Trace, generate more detailed and frequent logs, which often negatively impact performance by increasing CPU usage, memory consumption, and the amount of reading and writing to the computer.  
- In production, a higher level like Warning or Error minimizes improves logging noise, focusing on critical issues while improving performance and conserving resources.

## Suggested Coding Exercise
- Have students add logging to their API

## Building toward CSTA Standards:
- Develop computational artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs (3A-AP-18) https://www.csteachers.org/page/standards
- Modify an existing program to add additional functionality and discuss intended and unintended implications (e.g., breaking other functionality) (3B-AP-22) https://www.csteachers.org/page/standards
- Develop guidelines that convey systematic troubleshooting strategies that others can use to identify and fix errors (3A-CS-03) https://www.csteachers.org/page/standards

## Resources
- https://learn.microsoft.com/en-us/dotnet/core/extensions/logging