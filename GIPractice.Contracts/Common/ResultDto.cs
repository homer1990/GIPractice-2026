namespace GIPractice.Contracts.Common;

public sealed record ResultDto<T>(
    T? Data,
    ErrorDto? Error)
{
    public bool IsSuccess => Error is null;
    public static ResultDto<T> Ok(T data) => new(data, null);
    public static ResultDto<T> Fail(ErrorDto error) => new(default, error);
}
