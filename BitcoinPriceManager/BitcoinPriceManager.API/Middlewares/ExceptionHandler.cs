namespace BitcoinPriceManager.API.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred");
            httpContext.Response.StatusCode = 500; // Internal server error
            await httpContext.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
        }
    }
}
