namespace GIPractice.Contracts.Auth;

public sealed record LoginResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    AuthMeDto User);
