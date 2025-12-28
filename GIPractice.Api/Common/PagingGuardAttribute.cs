using GIPractice.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GIPractice.Api.Common;

[AttributeUsage(AttributeTargets.Method)]
public sealed class PagingGuardAttribute : ActionFilterAttribute
{
    public int MaxPageSize { get; init; } = 500;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg is null) continue;

            var pagingProp = arg.GetType().GetProperty("Paging");
            if (pagingProp?.GetValue(arg) is not PagedRequestDto paging) continue;

            if (paging.Page < 1)
            {
                context.Result = new BadRequestObjectResult(ResultDto<bool>.Fail("validation", "Page must be >= 1."));
                return;
            }

            if (paging.PageSize < 1 || paging.PageSize > MaxPageSize)
            {
                context.Result = new BadRequestObjectResult(
                    ResultDto<bool>.Fail("validation", $"PageSize must be between 1 and {MaxPageSize}."));
                return;
            }
        }
    }
}
