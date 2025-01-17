In today's lesson we'll look at one-way hashing.  One-way hashing is a process where input data is transformed into a fixed-size string of characters, which cannot be reversed to reveal the original data. It's used in APIs for securely storing sensitive information because even if the hash is exposed, the original data remains protected.  This ensures data integrity and enhances security by preventing unauthorized access to plaintext data. This process is often used to securely transform passwords before they are stored in a database. 

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
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly string issuer;
    private readonly string audience;
    private readonly string secret;
    private readonly IUserRepository userRepository;
    private readonly ValueHasher passwordHasher;
    private readonly IMapper mapper;

    public AuthenticationController(IConfiguration configuration, 
                                    IUserRepository userRepository, 
                                    ValueHasher passwordHasher, 
                                    IMapper mapper)
    {
        issuer = configuration["Jwt:Issuer"];
        audience = configuration["Jwt:Audience"];
        secret = configuration["Jwt:Secret"];
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        var existingUser = userRepository.FindByUsername(userDto.Username);
        if (existingUser != null)
        {
            return BadRequest("Username is already taken.");
        }

        var user = mapper.Map<User>(userDto);
        user.HashedPassword = passwordHasher.HashPassword(userDto.Password);

        userRepository.Save(user);
        return Ok("User registered successfully.");
    }

    [HttpPost("token")]
    public IActionResult Token([FromBody] UserDto userDto)
    {
        var user = userRepository.FindByUsername(userDto.Username);
        if (user == null || !passwordHasher.VerifyPassword(user.HashedPassword, userDto.Password))
        {
            return Unauthorized("Invalid username or password.");
        }

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
public class UserDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
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
        CreateMap<UserDto, User>();
    }
}
```

``` cs
using System.Security.Cryptography;
using System.Text;

public class ValueHasher
{
    public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        var hashedInputPassword = HashPassword(password);
        return hashedPassword == hashedInputPassword;
    }
}
```

``` cs
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public string Role { get; set; }
}
```

``` cs
using Microsoft.EntityFrameworkCore;

namespace MyFirstApi.Models
{
    public class WeatherForecastDbContext : DbContext
    {
        public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options): base(options) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public DbSet<WeatherAlert> WeatherAlerts { get; set; }
        public DbSet<WeatherComment> WeatherComments { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<CityWeatherForecast> CityWeatherForecasts { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
```

``` cs
namespace MyFirstApi.Models
{
    public interface IUserRepository : IWriteRepository<int, User> 
    {
        User FindByUsername(string username);
    }
}
```

``` cs
namespace MyFirstApi.Models
{
    using System;

    public class UserRepository : IUserRepository
    {
        private WeatherForecastDbContext context;

        public UserRepository(WeatherForecastDbContext context)
        {
            this.context = context;
        }

        public User Save(User user)
        {
            context.Users.Add(user);
            
            context.SaveChanges();
            return user;
        }

        public User Update(int id, User user)
        {
            user.Id = id;
            context.Users.Update(user);
            context.SaveChanges();
            return user;
        }


        public User FindByUsername(string username)
        {
            return context.Users.FirstOrDefault(u => u.Username == username);
        }

        public User RemoveById(int id)
        {
            var user = context.Users.Find(id);
            context.Users.Remove(user);
            context.SaveChanges();
            return user;
        }

        public int Count()
        {
            return context.Users.Count();
        }
    }
}
```

``` cs
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;
using MyFirstApi.Services;
using Microsoft.AspNetCore.Authentication;


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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeatherForecastDbContext>();
    db.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```


In the coding exercise you will create a one way hash.

## Main Points
- One-way hashing is a process where input data is transformed into a fixed-size string of characters, which cannot be reversed to reveal the original data.
- This process is often used to securely transform passwords before they are stored in a database.

## Suggested Coding Exercise
- Have students hash a value before they store it in the database.

## Building toward CSTA Standards:
- Give examples to illustrate how sensitive data can be affected by malware and other attacks (3A-NI-05) https://www.csteachers.org/page/standards
- Recommend security measures to address various scenarios based on factors such as efficiency, feasibility, and ethical impacts (3A-NI-06) https://www.csteachers.org/page/standards
- Compare various security measures, considering tradeoffs between the usability and security of a computing system (3A-NI-07) https://www.csteachers.org/page/standards
- Explain tradeoffs when selecting and implementing cybersecurity recommendations (3A-NI-08) https://www.csteachers.org/page/standards
- Compare ways software developers protect devices and information from unauthorized access (3B-NI-04) https://www.csteachers.org/page/standards
- Explain security issues that might lead to compromised computer programs (3B-AP-18) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Cryptographic_hash_function
