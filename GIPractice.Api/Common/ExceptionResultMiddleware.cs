using GIPractice.Contracts.Common;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;

namespace GIPractice.Api.Common;

public sealed class ExceptionResultMiddleware(
    RequestDelegate next,
    ILogger<ExceptionResultMiddleware> log,
    IHostEnvironment env)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            if (ctx.Response.HasStarted) throw;

            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ctx.Response.ContentType = "application/json";

            var traceId = ctx.TraceIdentifier;

            // Always log server exceptions. Details returned to client only in Development.
            log.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

            var dto = ResultDto<object?>.Fail(
                ErrorCodes.Unexpected,
                "Unexpected error.",
                details: env.IsDevelopment() ? ex.ToString() : null,
                validationErrors: null,
                traceId: traceId);

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(dto));
        }
    }
}
