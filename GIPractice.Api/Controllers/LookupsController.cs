using AutoMapper;
using GIPractice.Api.Models.Lookups;
using GIPractice.Core.Entities;
using GIPractice.Core.Enums;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // ORGANS
    // GET /api/lookups/organs
    [HttpGet("organs")]
    public async Task<ActionResult<IEnumerable<OrganLookupDto>>> GetOrgans([FromQuery] string? lang = null)
    {
        var organs = await _db.Organs
            .AsNoTracking()
            .OrderBy(o => o.Code)
            .ToListAsync();

        var dto = _mapper.Map<List<OrganLookupDto>>(organs);

        if (!string.IsNullOrWhiteSpace(lang))
        {
            lang = lang.ToLowerInvariant();

            var translations = await _db.LocalizationStrings
                .Where(l => l.Domain == "Organ" && l.Language == lang)
                .AsNoTracking()
                .ToListAsync();

            var dict = translations.ToDictionary(
                l => l.Code,
                l => l.Text,
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in dto)
            {
                if (dict.TryGetValue(item.Code, out var text))
                    item.DefaultName = text;
            }
        }

        return Ok(dto);
    }


    // ORGAN AREAS
    // GET /api/lookups/organareas
    [HttpGet("organareas")]
    public async Task<ActionResult<IEnumerable<OrganAreaLookupDto>>> GetOrganAreas([FromQuery] string? lang = null)
    {
        var organAreas = await _db.OrganAreas
            .AsNoTracking()
            .Include(oa => oa.OrganAreaOrgans)
                .ThenInclude(oao => oao.Organ)
            .OrderBy(oa => oa.Code)
            .ToListAsync();

        var dto = _mapper.Map<List<OrganAreaLookupDto>>(organAreas);

        if (!string.IsNullOrWhiteSpace(lang))
        {
            lang = lang.ToLowerInvariant();

            var translations = await _db.LocalizationStrings
                .Where(l => l.Domain == "OrganArea" && l.Language == lang)
                .AsNoTracking()
                .ToListAsync();

            var dict = translations.ToDictionary(
                l => l.Code,
                l => l.Text,
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in dto)
            {
                if (dict.TryGetValue(item.Code, out var text))
                    item.DefaultName = text;
            }
        }

        return Ok(dto);
    }

    // ----------------------------------------
    // FINDINGS
    // GET /api/lookups/findings
    // ----------------------------------------
    [HttpGet("findings")]
    public async Task<ActionResult<IEnumerable<FindingLookupDto>>> GetFindings()
    {
        var findings = await _db.Findings
            .AsNoTracking()
            .OrderBy(f => f.Code)
            .ThenBy(f => f.Name)
            .ToListAsync();

        var dto = _mapper.Map<List<FindingLookupDto>>(findings);
        return Ok(dto);
    }

    // ----------------------------------------
    // PREPARATION PROTOCOLS
    // GET /api/lookups/preparation-protocols
    // ----------------------------------------
    [HttpGet("preparation-protocols")]
    public async Task<ActionResult<IEnumerable<PreparationProtocolLookupDto>>> GetPreparationProtocols()
    {
        var protocols = await _db.PreparationProtocols
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.EndoscopyType)
            .ThenBy(p => p.Name)
            .ToListAsync();

        var dto = _mapper.Map<List<PreparationProtocolLookupDto>>(protocols);
        return Ok(dto);
    }

    // ----------------------------------------
    // ENDOSCOPY TYPES (enum)
    // GET /api/lookups/endoscopy-types
    // ----------------------------------------
    [HttpGet("endoscopy-types")]
    public async Task<ActionResult<IEnumerable<EnumLookupDto>>> GetEndoscopyTypes([FromQuery] string? lang = null)
    {
        // 1) Base list from enum
        var values = Enum.GetValues(typeof(EndoscopyType))
            .Cast<EndoscopyType>()
            // If you have a 'None = 0' type, you can filter it out:
            //.Where(t => t != EndoscopyType.None)
            .Select(t => new EnumLookupDto
            {
                Value = (int)t,
                Name = t.ToString()      // default English label == enum name
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(lang))
        {
            lang = lang.ToLowerInvariant();

            // Domain: "EndoscopyType", Code = enum name
            var translations = await _db.LocalizationStrings
                .Where(l => l.Domain == "EndoscopyType" && l.Language == lang)
                .AsNoTracking()
                .ToListAsync();

            var dict = translations.ToDictionary(
                l => l.Code,        // Code = enum name
                l => l.Text,
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in values)
            {
                // enumName = current English Name
                var enumName = item.Name;
                if (dict.TryGetValue(enumName, out var text))
                    item.Name = text;    // overwrite with localized label
            }
        }

        return Ok(values);
    }
    // ----------------------------------------
    // TEST TYPES (enum)
    // GET /api/lookups/test-types
    // ----------------------------------------
    [HttpGet("test-types")]
    public async Task<ActionResult<IEnumerable<EnumLookupDto>>> GetTestTypes([FromQuery] string? lang = null)
    {
        var values = Enum.GetValues(typeof(TestType))
            .Cast<TestType>()
            .Where(t => t != TestType.None)
            .Select(t => new EnumLookupDto
            {
                Value = (int)t,
                Name = t.ToString()
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(lang))
        {
            lang = lang.ToLowerInvariant();

            var translations = await _db.LocalizationStrings
                .Where(l => l.Domain == "TestType" && l.Language == lang)
                .AsNoTracking()
                .ToListAsync();

            var dict = translations.ToDictionary(
                l => l.Code,
                l => l.Text,
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in values)
            {
                var enumName = item.Name;
                if (dict.TryGetValue(enumName, out var text))
                    item.Name = text;
            }
        }

        return Ok(values);
    }
    // ----------------------------------------
    // OPERATION TYPES (enum)
    // GET /api/lookups/operation-types
    // ----------------------------------------
    [HttpGet("operation-types")]
    public async Task<ActionResult<IEnumerable<EnumLookupDto>>> GetOperationTypes([FromQuery] string? lang = null)
    {
        var values = Enum.GetValues(typeof(OperationType))
            .Cast<OperationType>()
            .Where(t => t != OperationType.None)
            .Select(t => new EnumLookupDto
            {
                Value = (int)t,
                Name = t.ToString()
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(lang))
        {
            lang = lang.ToLowerInvariant();

            var translations = await _db.LocalizationStrings
                .Where(l => l.Domain == "OperationType" && l.Language == lang)
                .AsNoTracking()
                .ToListAsync();

            var dict = translations.ToDictionary(
                l => l.Code,
                l => l.Text,
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in values)
            {
                var enumName = item.Name;
                if (dict.TryGetValue(enumName, out var text))
                    item.Name = text;
            }
        }

        return Ok(values);
    }
}
