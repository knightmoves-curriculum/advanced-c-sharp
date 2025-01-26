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