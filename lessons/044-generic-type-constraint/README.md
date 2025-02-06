In today's lesson we'll look at default interface methods. A default interface method in C# is a method inside an interface that has a default implementation, allowing new methods to be added to interfaces without breaking existing implementations. This feature was first introduced in C# 8 (2019) and enables interfaces to provide behavior while still allowing implementing classes to override the method if needed. Traditionally, interfaces in C# could only declare method signatures without implementations, requiring all implementing classes to define the behavior, but default methods change this by allowing optional method bodies.

``` cs
namespace MyFirstApi.Models
{
    public interface IUserRepository : IWriteRepository<int, User> 
    {
        User FindByUsername(string username);
        void LogUserName(string username)
        {
            Console.WriteLine("Log " + username);
        }
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
        userRepository.LogUserName(userDto.Username);
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

`dotnet run`

In the coding exercise you will use a default interface method.

## Main Points
- A default interface method in C# is a method inside an interface that has a default implementation, allowing new methods to be added to interfaces without breaking existing implementations.
- Default interface methods were first introduced in C# 8.
- Before C# 8 interfaces could only declare method signatures without implementations, requiring all implementing classes to define the behavior.

## Suggested Coding Exercise
- Have students use Null Coalescing.

## Building toward CSTA Standards:
None

## Resources
- https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/default-interface-methods