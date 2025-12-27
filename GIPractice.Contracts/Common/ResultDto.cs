namespace GIPractice.Contracts.Common;

public sealed record ResultDto<T>(
    T? Data,
    ErrorDto? Error)
{
    public bool IsSuccess => Error is null;
    public static ResultDto<T> Ok(T data) => new(data, null);
    public static ResultDto<T> Fail(ErrorDto error) => new(default, error);

    public static ResultDto<T> Fail(
        string code,
        string message,
        IReadOnlyDictionary<string, IReadOnlyList<string>>? details = null)
        => new(default, new ErrorDto(code, message, details));
}
