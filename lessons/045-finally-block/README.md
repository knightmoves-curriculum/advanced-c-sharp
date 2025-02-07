In today's lesson we'll look at a finally block. A `finally` block is executed after the try block completes, regardless of whether an exception was thrown or caught, ensuring that cleanup code (such as releasing resources) always runs.

``` cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _secret;

    public JwtAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration) : base(options, logger, encoder, clock)
    {
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
        _secret = configuration["Jwt:Secret"];
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Missing Authorization header");
        }

        try
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ").Last();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization header");
        }
        finally
        {
            base.Logger.LogInformation("Hit finally block");
        }
    }
}
```

`dotnet run`

**Example in ValueEncryptor.cs**

A using block is a construct that ensures an object implementing IDisposable is automatically disposed of at the end of the block, even if an exception occurs. It is similar to a finally block because both guarantee resource cleanup, but using is specifically designed for IDisposable objects and is more concise, eliminating the need for explicit disposal in finally.


In the coding exercise you will use a finally block.

## Main Points
- A `finally` block is executed after the try block completes, regardless of whether an exception was thrown or caught, ensuring that cleanup code (such as releasing resources) always runs.
- A `using` block is similar to a `finally` block because both guarantee resource cleanup, but `using` is specifically designed for IDisposable objects and is more concise, eliminating the need for explicit disposal in `finally`.

## Suggested Coding Exercise
- Have students use a generic type constraint

## Building toward CSTA Standards:
None

## Resources
- https://learn.microsoft.com/en-us/dotnet/standard/exceptions/how-to-use-finally-blocks