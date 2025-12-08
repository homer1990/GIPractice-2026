namespace GIPractice.Core.Services;

public interface ILocalizationService
{
    Task<string> GetValueAsync(string table, string field, string cultureName, CancellationToken cancellationToken = default);
}
