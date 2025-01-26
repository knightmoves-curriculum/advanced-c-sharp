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

            return BadRequest(message);
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