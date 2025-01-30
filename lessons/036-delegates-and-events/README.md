In today's lesson we'll look at delegates and events.  Delegates in C# are references to methods with a specific signature, ensuring that the method being referenced matches the expected parameters which enables dynamic behavior. Events, built on delegates, provide a way for a class to notify multiple subscribers when a specific action occurs, promoting loose coupling between components. In an API, using delegates and events enhances extensibility by allowing services to react to changes without directly modifying the core logic. Extensibility is the ability of a system to accommodate future growth, modifications, or enhancements with minimal changes to existing code.

``` cs
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class ValueEncryptor
{
    public delegate void DecryptionHandler(string cipherText, string plaintext);
    public event DecryptionHandler? ValueDecrypted;
    private static string key;
    private static string iv;

    public ValueEncryptor(IConfiguration configuration)
    {
        key = configuration["AES:Key"];
        iv = configuration["AES:InitializationVector"];
    }

    public string Encrypt(string plainText)
    {
        if (key.Length != 32 || iv.Length != 16)
        {
            throw new ArgumentException("Key must be 32 bytes and IV must be 16 bytes long.");
        }

        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(key);
        aesAlg.IV = Encoding.UTF8.GetBytes(iv);

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        byte[] encrypted;
        using (MemoryStream msEncrypt = new MemoryStream())
        {
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
            }

            encrypted = msEncrypt.ToArray();
        }

        return Convert.ToBase64String(encrypted);
    }

    public string Decrypt(string cipherText)
    {
        if (key.Length != 32 || iv.Length != 16)
        {
            throw new ArgumentException("Key must be 32 bytes and IV must be 16 bytes long.");
        }

        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(key);
        aesAlg.IV = Encoding.UTF8.GetBytes(iv);

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        string plaintext;
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
        {
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    plaintext = srDecrypt.ReadToEnd();
                }
            }
        }

        ValueDecrypted.Invoke(cipherText, plaintext);

        return plaintext;
    }
}
```

``` cs
using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class DecryptionAuditService
    {
        private readonly ILogger<DecryptionAuditService> logger;

        public DecryptionAuditService(ILogger<DecryptionAuditService> logger)
        {
            this.logger = logger;
        }
        
        public void OnValueDecrypted(string cipherText, string plaintext)
        {
            Console.WriteLine($"[Audit] Decrypted: {cipherText} to {plaintext}");
        }
    }
}
```

``` cs
using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class DecryptionLoggingService
    {

        private readonly ILogger<DecryptionLoggingService> logger;

        public DecryptionLoggingService(ILogger<DecryptionLoggingService> logger)
        {
            this.logger = logger;
        }

        public void OnValueDecrypted(string cipherText, string plaintext)
        {
            Console.WriteLine($"[Logging] Decrypted: {cipherText} to {plaintext}");
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

`dotnet run`

It's important to note that this delegate model follows the observer design pattern, which enables a subscriber to register with and receive notifications from a provider.  The observer design pattern enables a subscriber to register with and receive notifications from a provider.

In the coding exercise you will use delegates and events.

## Main Points
- Delegates in C# are references to methods with a specific signature, ensuring that the method being referenced matches the expected parameters which enables dynamic behavior. 
- Events, built on delegates, provide a way for a class to notify multiple subscribers when a specific action occurs, promoting loose coupling between components. 
- Using delegates and events enhances extensibility by allowing services to react to changes without directly modifying the core logic.
- Extensibility is the ability of a system to accommodate future growth, modifications, or enhancements with minimal changes to existing code.
- The observer design pattern enables a subscriber to register with and receive notifications from a provider.

## Suggested Coding Exercise
- Have students add pagination to their API.  **Stick with singletons at this point because the subscribers can be configured once in the Program.cs file**

## Building toward CSTA Standards:
- Decompose problems into smaller components through systematic analysis, using constructs such as procedures, modules, and/or objects (3A-AP-17) https://www.csteachers.org/page/standards
- Create artifacts by using procedures within a program, combinations of data and procedures, or independent but interrelated programs (3A-AP-18) https://www.csteachers.org/page/standards
- Design and iteratively develop computational artifacts for practical intent, personal expression, or to address a societal issue by using events to initiate instructions (3A-AP-16) https://www.csteachers.org/page/standards

## Resources
- https://learn.microsoft.com/en-us/dotnet/standard/events/
- https://en.wikipedia.org/wiki/Extensibility
- https://en.wikipedia.org/wiki/Observer_pattern
