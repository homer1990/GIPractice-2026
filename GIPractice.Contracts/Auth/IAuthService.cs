using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Auth;

public interface IAuthService
{
    Task<ResultDto<LoginResponseDto>> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<AuthMeDto>> MeAsync(CancellationToken cancellationToken = default);
}
