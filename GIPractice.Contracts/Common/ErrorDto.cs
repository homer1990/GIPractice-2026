namespace GIPractice.Contracts.Common;

public sealed record ErrorDto(
    string Code,
    string Message,
    object? Details = null,
    IReadOnlyDictionary<string, string[]>? ValidationErrors = null,
    string? TraceId = null);
