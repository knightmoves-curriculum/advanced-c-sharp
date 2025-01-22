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