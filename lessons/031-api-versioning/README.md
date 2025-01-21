In today's lesson we'll look at API Versioning.  API versioning is the practice of creating and managing multiple versions of an API to maintain backward compatibility while introducing new features or changes. It solves the problem of breaking changes by ensuring that clients using older versions of the API are not affected when newer versions are released. For example, if a field like "social security number" is changed from a single string to three separate parts in a new version of the API, clients relying on the previous single string format would break, causing issues for those using the older version without the update.

`dotnet add package Microsoft.AspNetCore.Mvc.Versioning`

``` cs
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;
using MyFirstApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeatherForecastDbContext>();
    db.Database.Migrate();
}


app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
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

    public AuthenticationV1Controller(IConfiguration configuration, 
                                    IUserRepository userRepository, 
                                    ValueHasher passwordHasher, 
                                    ValueEncryptor valueEncryptor,
                                    IMapper mapper)
    {
        issuer = configuration["Jwt:Issuer"];
        audience = configuration["Jwt:Audience"];
        secret = configuration["Jwt:Secret"];
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.valueEncryptor = valueEncryptor;
        this.mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDtoV1 userDto)
    {
        var existingUser = userRepository.FindByUsername(userDto.Username);
        if (existingUser != null)
        {
            return BadRequest("Username is already taken.");
        }

        var user = mapper.Map<User>(userDto);

        string hashPassword = passwordHasher.HashPassword(userDto.Password);
        Console.WriteLine("Hashed Password: " + hashPassword);
        user.HashedPassword = hashPassword;

        string encryptedSocialSecurityNumber = valueEncryptor.Encrypt(userDto.SocialSecurityNumber);
        Console.WriteLine("Encrypted Social Security Number: " + encryptedSocialSecurityNumber);
        user.EncryptedSocialSecurityNumber = encryptedSocialSecurityNumber;

        userRepository.Save(user);
        return Ok("User registered successfully.");
    }

    [HttpPost("token")]
    public IActionResult Token([FromBody] UserDtoV1 userDto)
    {
        var user = userRepository.FindByUsername(userDto.Username);
        if (user == null || !passwordHasher.VerifyPassword(user.HashedPassword, userDto.Password))
        {
            return Unauthorized("Invalid username or password.");
        }

        string socialSecurityNumber = valueEncryptor.Decrypt(user.EncryptedSocialSecurityNumber);
        Console.WriteLine("Decrypted Social Security Number: " + socialSecurityNumber);

        string token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

URI Path Versioning is a method of versioning an API by including the version number directly in the URL path. This approach makes it clear which version of the API is being requested and allows for easy management of multiple API versions through distinct paths.

``` cs
public class UserDtoV1
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string SocialSecurityNumber { get; set; }
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
[ApiVersion("2.0")]
[Route("v{version:apiVersion}/authentication")]
public class AuthenticationV2Controller : ControllerBase
{
    private readonly string issuer;
    private readonly string audience;
    private readonly string secret;
    private readonly IUserRepository userRepository;
    private readonly ValueHasher passwordHasher;
    private readonly ValueEncryptor valueEncryptor;
    private readonly IMapper mapper;

    public AuthenticationV2Controller(IConfiguration configuration, 
                                    IUserRepository userRepository, 
                                    ValueHasher passwordHasher, 
                                    ValueEncryptor valueEncryptor,
                                    IMapper mapper)
    {
        issuer = configuration["Jwt:Issuer"];
        audience = configuration["Jwt:Audience"];
        secret = configuration["Jwt:Secret"];
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.valueEncryptor = valueEncryptor;
        this.mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDtoV2 userDto)
    {
        var existingUser = userRepository.FindByUsername(userDto.Username);
        if (existingUser != null)
        {
            return BadRequest("Username is already taken.");
        }

        var user = mapper.Map<User>(userDto);

        string hashPassword = passwordHasher.HashPassword(userDto.Password);
        Console.WriteLine("Hashed Password: " + hashPassword);
        user.HashedPassword = hashPassword;

        string encryptedSocialSecurityNumber = valueEncryptor.Encrypt(ExtractSocialSecurityNumberString(userDto));
        Console.WriteLine("Encrypted Social Security Number: " + encryptedSocialSecurityNumber);
        user.EncryptedSocialSecurityNumber = encryptedSocialSecurityNumber;

        userRepository.Save(user);
        return Ok("User registered successfully.");
    }

    private  string ExtractSocialSecurityNumberString(UserDtoV2 userDto)
    {
        return userDto.SocialSecurityNumber.AreaNumber + "-" + userDto.SocialSecurityNumber.GroupNumber + "-" + userDto.SocialSecurityNumber.SerialNumber;
    }

    [HttpPost("token")]
    public IActionResult Token([FromBody] UserDtoV2 userDto)
    {
        var user = userRepository.FindByUsername(userDto.Username);
        if (user == null || !passwordHasher.VerifyPassword(user.HashedPassword, userDto.Password))
        {
            return Unauthorized("Invalid username or password.");
        }

        string socialSecurityNumber = valueEncryptor.Decrypt(user.EncryptedSocialSecurityNumber);
        Console.WriteLine("Decrypted Social Security Number: " + socialSecurityNumber);

        string token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

``` cs
using System.Text.Json.Serialization;

public class UserDtoV2
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public SocialSecurityNumber SocialSecurityNumber { get; set; }
}
```

``` cs
public class SocialSecurityNumber
{
    public string AreaNumber { get; set; }
    public string GroupNumber { get; set; }
    public string SerialNumber { get; set; }
}
```

``` cs
using AutoMapper;

public class WeatherForecastProfile : Profile
{
    public WeatherForecastProfile()
    {
        CreateMap<WeatherForecastDto, WeatherForecast>()
            .ForMember(dest => dest.TemperatureC, 
                       opt => opt.MapFrom(src => (src.TemperatureF - 32) * 5 / 9))
            .ForMember(dest => dest.Alert, 
                       opt => opt.MapFrom(src => src.Alert != null 
                                                 ? new WeatherAlert { AlertMessage = src.Alert } 
                                                 : null))
            .ForMember(dest => dest.Comments, 
                       opt => opt.MapFrom(src => src.Comments != null 
                                                 ? src.Comments.Select(c => new WeatherComment { CommentMessage = c }).ToList() 
                                                 : new List<WeatherComment>()));
        CreateMap<CityDto, City>();
        CreateMap<UserDtoV1, User>();
        CreateMap<UserDtoV2, User>();
    }
}
```

`dotnet run`

``` json
{
    "username": "TestUser",
    "password": "TestPassword",
    "role": "Admin",
    "socialSecurityNumber": {
        "areaNumber": "123",
        "groupNumber": "45",
        "serialNumber": "6789"
    }
}
```

In the coding exercise you will version your api.

## Main Points
- API versioning is the practice of creating and managing multiple versions of an API to maintain backward compatibility while introducing new features or changes.
- API versioning solves the problem of breaking changes by ensuring that clients using older versions of the API are not affected when newer versions are released.
- URI Path Versioning is a method of versioning an API by including the version number directly in the URL path.


## Suggested Coding Exercise
- Have students version thier API

## Building toward CSTA Standards:
- Design and iteratively develop computational artifacts for practical intent, personal expression, or to address a societal issue by using events to initiate instructions (3A-AP-16) https://www.csteachers.org/page/standards
- Demonstrate code reuse by creating programming solutions using libraries and APIs (3B-AP-16) https://www.csteachers.org/page/standards
- Plan and develop programs for broad audiences using a software life cycle process (3B-AP-17) https://www.csteachers.org/page/standards
- Use version control systems, integrated development environments (IDEs), and collaborative tools and practices (code documentation) in a group software project (3B-AP-20) https://www.csteachers.org/page/standards
- Modify an existing program to add additional functionality and discuss intended and unintended implications (e.g., breaking other functionality) (3B-AP-22) https://www.csteachers.org/page/standards

## Resources
- https://github.com/dotnet/aspnet-api-versioning
