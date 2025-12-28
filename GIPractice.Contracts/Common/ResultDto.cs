namespace GIPractice.Contracts.Common;

public sealed record ResultDto<T>(
    bool IsSuccess,
    T? Value,
    ErrorDto? Error)
{
    public static ResultDto<T> Ok(T value)
        => new(IsSuccess: true, Value: value, Error: null);

    // Allows: ResultDto<T>.Fail(new ErrorDto(...))
    public static ResultDto<T> Fail(ErrorDto error)
        => new(IsSuccess: false, Value: default, Error: error);

    // Allows: ResultDto<T>.Fail("not_found")
    public static ResultDto<T> Fail(string code)
        => new(IsSuccess: false, Value: default, Error: new ErrorDto(code, code));

    // Allows: ResultDto<T>.Fail("not_found", "Patient not found.")
    public static ResultDto<T> Fail(string code, string message)
        => new(IsSuccess: false, Value: default, Error: new ErrorDto(code, message));

    public static ResultDto<T> Fail(
        string code,
        string message,
        object? details = null,
        IReadOnlyDictionary<string, string[]>? validationErrors = null,
        string? traceId = null)
        => new(
            IsSuccess: false,
            Value: default,
            Error: new ErrorDto(code, message, details, validationErrors, traceId));
}
