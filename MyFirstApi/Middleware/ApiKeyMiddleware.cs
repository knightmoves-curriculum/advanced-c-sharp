using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class ApiKeyMiddleware
{
    private const string APIKEYNAME = "X-Api-Key";
    private readonly RequestDelegate _next;
    private string apiKey;
    private readonly ILogger<ApiKeyMiddleware> logger;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        apiKey = configuration["ApiKey"];
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogDebug("Starting middleware");
        if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key was not provided.");
            return;
        }

        if (!apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }
        logger.LogDebug("Calling next middleware");
        await _next(context);
    }
}