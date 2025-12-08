using System;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Client.Core;

namespace GIPractice.ApiClient;

public class InMemoryTokenService : ITokenService
{
    private readonly object _gate = new();

    public string? AccessToken { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }

    public Task<bool> EnsureValidTokenAsync(CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            if (AccessToken is null)
            {
                return Task.FromResult(false);
            }

            if (ExpiresAt is null || ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
            {
                return Task.FromResult(true);
            }

            AccessToken = null;
            ExpiresAt = null;
            return Task.FromResult(false);
        }
    }

    public Task<bool> SignInAsync(string userName, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult(false);
        }

        lock (_gate)
        {
            AccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1);
        }

        return Task.FromResult(true);
    }

    public Task SignOutAsync()
    {
        lock (_gate)
        {
            AccessToken = null;
            ExpiresAt = null;
        }

        return Task.CompletedTask;
    }
}
