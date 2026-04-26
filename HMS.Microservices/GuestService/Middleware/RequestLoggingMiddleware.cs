namespace GuestService.Middleware;
 


 // custom middleware 
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);
        await next(context);
        logger.LogInformation("Response status: {StatusCode}", context.Response.StatusCode);
    }
}
