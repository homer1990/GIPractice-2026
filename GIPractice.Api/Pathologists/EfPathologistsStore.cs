using System.Text.Json;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathologists;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Pathologists;

public sealed class EfPathologistsStore : IPathologistsStore
{
    private readonly AppDbContext _db;

    public EfPathologistsStore(AppDbContext db) => _db = db;

    public async Task<ResultDto<PagedResultDto<PathologistListItemDto>>> SearchAsync(
        PathologistSearchRequestDto request,
        CancellationToken ct = default)
    {
        var paging = request.Paging ?? new PagedRequestDto();

        if (paging.Page < 1)
            return ResultDto<PagedResultDto<PathologistListItemDto>>.Fail("invalid", "Page must be >= 1.");

        if (paging.PageSize is < 1 or > 500)
            return ResultDto<PagedResultDto<PathologistListItemDto>>.Fail("invalid", "PageSize must be between 1 and 500.");

        IQueryable<Pathologist> q = _db.Pathologists.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var term = request.Name.Trim();
            q = q.Where(x => EF.Functions.Like(x.Name, $"%{term}%"));
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .Select(x => new PathologistListItemDto(
                new PathologistId(x.Id),
                x.Name,
                x.Email,
                x.PhoneNumber))
            .ToListAsync(ct);

        return ResultDto<PagedResultDto<PathologistListItemDto>>.Ok(
            new PagedResultDto<PathologistListItemDto>(items, total, paging.Page, paging.PageSize));
    }

    public async Task<ResultDto<PathologistDto>> GetAsync(PathologistId id, CancellationToken ct = default)
    {
        var p = await _db.Pathologists
            .AsNoTracking()
            .Where(x => x.Id == id.Value)
            .Select(x => new PathologistDto(
                new PathologistId(x.Id),
                x.Name,
                x.Address,
                x.Email,
                x.PhoneNumber,
                x.PricingPlanJson,
                x.RowVersion))
            .FirstOrDefaultAsync(ct);

        return p is null
            ? ResultDto<PathologistDto>.Fail("not_found", "Pathologist not found.")
            : ResultDto<PathologistDto>.Ok(p);
    }

    public async Task<ResultDto<PathologistId>> CreateAsync(PathologistUpsertRequestDto request, CancellationToken ct = default)
    {
        if (request.Id is not null)
            return ResultDto<PathologistId>.Fail("invalid", "Id must be null when creating a pathologist.");

        if (string.IsNullOrWhiteSpace(request.Name))
            return ResultDto<PathologistId>.Fail("validation", "Name is required.");

        if (string.IsNullOrWhiteSpace(request.PricingPlanJson))
            return ResultDto<PathologistId>.Fail("validation", "PricingPlanJson is required.");

        if (!IsValidJson(request.PricingPlanJson))
            return ResultDto<PathologistId>.Fail("validation", "PricingPlanJson must be valid JSON.");

        var entity = new Pathologist
        {
            Name = TrimMax(request.Name, 200),
            Address = TrimMaxNullable(request.Address, 500),
            Email = TrimMaxNullable(request.Email, 200),
            PhoneNumber = TrimMaxNullable(request.PhoneNumber, 50),
            PricingPlanJson = request.PricingPlanJson.Trim(),
        };

        _db.Pathologists.Add(entity);

        await _db.SaveChangesAsync(ct);

        return ResultDto<PathologistId>.Ok(new PathologistId(entity.Id));
    }

    public async Task<ResultDto<bool>> UpdateAsync(PathologistUpsertRequestDto request, CancellationToken ct = default)
    {
        if (request.Id is null)
            return ResultDto<bool>.Fail("invalid", "Id is required when updating a pathologist.");

        var id = request.Id.Value.Value;

        var entity = await _db.Pathologists.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Pathologist not found.");

        if (request.RowVersion is not null && !request.RowVersion.SequenceEqual(entity.RowVersion))
            return ResultDto<bool>.Fail("conflict", "RowVersion conflict.");

        if (string.IsNullOrWhiteSpace(request.Name))
            return ResultDto<bool>.Fail("validation", "Name is required.");

        if (string.IsNullOrWhiteSpace(request.PricingPlanJson))
            return ResultDto<bool>.Fail("validation", "PricingPlanJson is required.");

        if (!IsValidJson(request.PricingPlanJson))
            return ResultDto<bool>.Fail("validation", "PricingPlanJson must be valid JSON.");

        entity.Name = TrimMax(request.Name, 200);
        entity.Address = TrimMaxNullable(request.Address, 500);
        entity.Email = TrimMaxNullable(request.Email, 200);
        entity.PhoneNumber = TrimMaxNullable(request.PhoneNumber, 50);
        entity.PricingPlanJson = request.PricingPlanJson.Trim();

        await _db.SaveChangesAsync(ct);
        return ResultDto<bool>.Ok(true);
    }

    public async Task<ResultDto<bool>> DeleteAsync(PathologistId id, CancellationToken ct = default)
    {
        var entity = await _db.Pathologists.FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Pathologist not found.");

        var hasParcels = await _db.PathologyParcels.AsNoTracking().AnyAsync(x => x.PathologistId == id.Value, ct);
        if (hasParcels)
            return ResultDto<bool>.Fail("validation", "Cannot delete pathologist: parcels exist.");

        var hasReports = await _db.PathologyReports.AsNoTracking().AnyAsync(x => x.PathologistId == id.Value, ct);
        if (hasReports)
            return ResultDto<bool>.Fail("validation", "Cannot delete pathologist: reports exist.");

        _db.Pathologists.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return ResultDto<bool>.Ok(true);
    }

    private static bool IsValidJson(string json)
    {
        try
        {
            using var _ = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static string TrimMax(string s, int max)
    {
        s = (s ?? string.Empty).Trim();
        return s.Length <= max ? s : s[..max];
    }

    private static string? TrimMaxNullable(string? s, int max)
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;

        s = s.Trim();
        return s.Length <= max ? s : s[..max];
    }
}
