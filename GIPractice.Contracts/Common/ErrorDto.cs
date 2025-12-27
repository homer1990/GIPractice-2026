namespace GIPractice.Contracts.Common;

public sealed record ErrorDto(
    string Code,
    string Message,
    IReadOnlyDictionary<string, IReadOnlyList<string>>? Details = null);
