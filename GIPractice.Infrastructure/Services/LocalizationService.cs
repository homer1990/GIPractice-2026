using GIPractice.Core.Entities;
using GIPractice.Core.Services;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Infrastructure.Services;

public class LocalizationService(AppDbContext dbContext) : ILocalizationService
{
    private readonly AppDbContext _db = dbContext;

    public async Task<string> GetValueAsync(string table, string field, string cultureName, CancellationToken cancellationToken = default)
    {
        var normalizedCulture = NormalizeCulture(cultureName);
        var normalizedTable = table.Trim();
        var normalizedField = field.Trim();

        var fieldName = await _db.FieldNames
            .Include(f => f.Localizations)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                f => f.TableName == normalizedTable && f.Field == normalizedField,
                cancellationToken);

        if (fieldName is null)
            return normalizedField;

        var translation = fieldName.Localizations
            .FirstOrDefault(l => l.CultureName == normalizedCulture);

        return translation?.Value ?? fieldName.DefaultText;
    }

    private static string NormalizeCulture(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
            return "en";

        var culture = cultureName.Trim();
        return culture.Length > 5 ? culture[..5].ToLowerInvariant() : culture.ToLowerInvariant();
    }
}
