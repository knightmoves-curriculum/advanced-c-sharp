using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

    public AuthenticationController(IConfiguration configuration)
    {
        issuer = configuration["Jwt:Issuer"];
        audience = configuration["Jwt:Audience"];
        secret = configuration["Jwt:Secret"];
    }

    [HttpPost("token")]
    public IActionResult Token([FromBody] TokenDto tokenDto)
    {
        if (string.IsNullOrEmpty(tokenDto.Role))
        {
            return BadRequest("Role is required.");
        }

        string token = GenerateJwtToken(tokenDto.Role);
        return Ok(new { token });
    }

    private string GenerateJwtToken(string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "mary@knightmove.org"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role)
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