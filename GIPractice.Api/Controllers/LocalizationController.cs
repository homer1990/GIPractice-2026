using GIPractice.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocalizationController(ILocalizationService localizationService) : ControllerBase
{
    private readonly ILocalizationService _localizationService = localizationService;

    [HttpGet("{table}/{field}")]
    public async Task<ActionResult<LocalizationResponseDto>> Get(string table, string field, [FromQuery] string? culture = null)
    {
        if (string.IsNullOrWhiteSpace(table) || string.IsNullOrWhiteSpace(field))
            return BadRequest("Missing table or field name");

        var value = await _localizationService.GetValueAsync(table, field, culture ?? "en", HttpContext.RequestAborted);

        return Ok(new LocalizationResponseDto
        {
            Table = table,
            Field = field,
            Culture = culture ?? "en",
            Value = value
        });
    }
}

public record LocalizationResponseDto
{
    public string Table { get; init; } = string.Empty;
    public string Field { get; init; } = string.Empty;
    public string Culture { get; init; } = "en";
    public string Value { get; init; } = string.Empty;
}
