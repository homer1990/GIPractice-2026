namespace GIPractice.Contracts.Auth;

public sealed record LoginRequestDto(
    string UserName,
    string Password);
