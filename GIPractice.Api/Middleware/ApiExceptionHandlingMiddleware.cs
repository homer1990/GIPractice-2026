using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GIPractice.Api.Middleware;

/// <summary>
/// Catches unhandled exceptions and returns RFC7807 ProblemDetails JSON.
/// </summary>
public class ApiExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ApiExceptionHandlingMiddleware> logger,
    IWebHostEnvironment env,
    IOptions<JsonOptions> jsonOptions)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger = logger;
    private readonly IWebHostEnvironment _env = env;
    private readonly JsonSerializerOptions _jsonOptions = jsonOptions.Value.JsonSerializerOptions;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client disconnected / request aborted: just log at Debug and do nothing.
            _logger.LogDebug("Request aborted by client: {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
            {
                // Response already started; nothing safe we can do
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Instance = context.Request.Path,
                // In dev, include the full exception; in prod, keep it generic
                Detail = _env.IsDevelopment()
                    ? ex.ToString()
                    : "An unexpected error occurred. Please try again or contact support."
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            var payload = JsonSerializer.Serialize(problem, _jsonOptions);
            await context.Response.WriteAsync(payload);
        }
    }
}

/// <summary>
/// Extension method to plug the middleware into the pipeline.
/// </summary>
public static class ApiExceptionHandlingExtensions
{
    public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiExceptionHandlingMiddleware>();
    }
}
