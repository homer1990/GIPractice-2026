using GIPractice.Contracts.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Common;

public static class ResultActionResultExtensions
{
    public static async Task<IActionResult> ToActionResultAsync<T>(
        this Task<ResultDto<T>> task,
        HttpContext httpContext)
        => (await task).ToActionResult(httpContext);

    public static IActionResult ToActionResult<T>(
        this ResultDto<T> result,
        HttpContext httpContext)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result);

        // Inject TraceId if missing
        var traceId = httpContext.TraceIdentifier;
        if (result.Error is { } err && string.IsNullOrWhiteSpace(err.TraceId))
        {
            result = result with { Error = err with { TraceId = traceId } };
        }

        var code = result.Error?.Code;

        return code switch
        {
            ErrorCodes.NotFound => new NotFoundObjectResult(result),
            ErrorCodes.Conflict => new ConflictObjectResult(result),
            ErrorCodes.Unauthorized => new UnauthorizedObjectResult(result),
            ErrorCodes.Forbidden => new ObjectResult(result) { StatusCode = StatusCodes.Status403Forbidden },
            ErrorCodes.Unexpected => new ObjectResult(result) { StatusCode = StatusCodes.Status500InternalServerError },

            // Validation/invalid/default → 400
            _ => new BadRequestObjectResult(result)
        };
    }
}
