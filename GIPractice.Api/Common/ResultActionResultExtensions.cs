using GIPractice.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Common;

public static class ResultActionResultExtensions
{
    public static IActionResult ToActionResult<T>(this ResultDto<T> res, HttpContext http)
    {
        if (res.IsSuccess)
            return new OkObjectResult(res);

        var status = MapStatusCode(res.Error);
        return new ObjectResult(res) { StatusCode = status };
    }

    public static async Task<IActionResult> ToActionResultAsync<T>(this Task<ResultDto<T>> task, HttpContext http)
        => (await task).ToActionResult(http);

    private static int MapStatusCode(ErrorDto? err)
        => err?.Code switch
        {
            "not_found" => StatusCodes.Status404NotFound,
            "conflict" => StatusCodes.Status409Conflict,
            "unauthorized" => StatusCodes.Status401Unauthorized,
            "forbidden" => StatusCodes.Status403Forbidden,
            "validation" or "invalid" => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };
}
