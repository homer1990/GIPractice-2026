using GIPractice.Contracts.Common;
using System.Net;
using System.Text.Json;

namespace GIPractice.Api.Common;

public sealed class ExceptionResultMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception)
        {
            if (ctx.Response.HasStarted) throw;

            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ctx.Response.ContentType = "application/json";

            var traceId = ctx.TraceIdentifier;

            var dto = ResultDto<object?>.Fail(
                ErrorCodes.Unexpected,
                "Unexpected error.",
                details: null,
                validationErrors: null,
                traceId: traceId);

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(dto));
        }
    }
}
