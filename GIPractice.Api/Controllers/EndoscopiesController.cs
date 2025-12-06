using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GIPractice.Api.Services;
using GIPractice.Core.Enums;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EndoscopiesController(AppDbContext db, IMapper mapper, IMediaStorageService storage) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;
    private readonly IMediaStorageService _storage = storage;

    // --------------------------------------------------------
    // GET Endoscopy (full details)
    // --------------------------------------------------------
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EndoscopyDto>> GetEndoscopy(int id)
    {
        var e = await _db.Endoscopies
            .Include(e => e.Observations).ThenInclude(o => o.Finding)
            .Include(e => e.Observations).ThenInclude(o => o.OrganArea)
            .Include(e => e.BiopsyBottles).ThenInclude(bb => bb.OrganAreas)
            .Include(e => e.Report)
            .Include(e => e.MediaFiles)   // <- or .Include(e => e.EndoscopyMedias)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (e == null)
            return NotFound();

        return Ok(_mapper.Map<EndoscopyDto>(e));
    }

    // --------------------------------------------------------
    // CREATE Endoscopy
    // --------------------------------------------------------
    [HttpPost]
    public async Task<ActionResult> CreateEndoscopy([FromBody] EndoscopyCreateDto dto)
    {
        // 1) Make sure the Visit exists and get its patient
        var visit = await _db.Visits
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == dto.VisitId);

        if (visit == null)
        {
            ModelState.AddModelError(nameof(dto.VisitId), "Visit does not exist.");
            return ValidationProblem(ModelState);
        }

        // 2) Build the endoscopy using *Visit.PatientId*, not client input
        var endo = new Endoscopy
        {
            VisitId = visit.Id,
            PatientId = visit.PatientId,
            Type = dto.Type,
            PerformedAtUtc = dto.PerformedAtUtc ?? visit.DateOfVisitUtc,
            IsUrgent = dto.IsUrgent,
            Notes = dto.Notes
        };

        _db.Endoscopies.Add(endo);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEndoscopy), new { id = endo.Id }, null);
    }


    // --------------------------------------------------------
    // ADD OBSERVATION
    // --------------------------------------------------------
    [HttpPost("{id:int}/observations")]
    public async Task<ActionResult> AddObservation(int id, [FromBody] ObservationCreateDto dto)
    {
        if (dto.EndoscopyId != id)
            return BadRequest("Endoscopy ID mismatch.");

        var obs = new Observation
        {
            EndoscopyId = id,
            FindingId = dto.FindingId,
            OrganAreaId = dto.OrganAreaId,
            Severity = dto.Severity,
            Density = dto.Density,
            Description = dto.Description,
            EndoscopyMediaId = dto.MediaId
        };

        _db.Observations.Add(obs);
        await _db.SaveChangesAsync();

        return Ok();
    }

    // --------------------------------------------------------
    // ADD BIOPSY BOTTLE
    // --------------------------------------------------------

    [HttpPost("{id:int}/biopsybottles")]
    public async Task<ActionResult> AddBiopsyBottle(int id, [FromBody] BiopsyBottleCreateDto dto)
    {
        if (dto.EndoscopyId != id)
            return BadRequest("Endoscopy ID mismatch.");

        var endoscopy = await _db.Endoscopies
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (endoscopy == null)
            return NotFound("Endoscopy not found.");

        var bottle = new BiopsyBottle
        {
            EndoscopyId = id,
            PatientId = endoscopy.PatientId,
            CollectedAtUtc = endoscopy.PerformedAtUtc,
            Label = dto.Label,
            Number = dto.Number
        };

        if (dto.OrganAreaIds.Count != 0)
        {
            var organAreas = await _db.OrganAreas
                .Where(oa => dto.OrganAreaIds.Contains(oa.Id))
                .ToListAsync();

            bottle.OrganAreas = organAreas;
        }

        _db.BiopsyBottles.Add(bottle);
        await _db.SaveChangesAsync();

        return Ok();
    }


    // --------------------------------------------------------
    // ADD REPORT
    // --------------------------------------------------------
    [HttpPost("{id:int}/report")]
    public async Task<ActionResult> AddReport(int id, [FromBody] ReportCreateDto dto)
    {
        if (dto.EndoscopyId != id)
            return BadRequest("Endoscopy ID mismatch.");

        var report = new Report
        {
            EndoscopyId = id,
            IsUrgent = dto.IsUrgent,
            FileId = dto.FileId,
            Summary = dto.Summary
        };

        _db.Reports.Add(report);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteEndoscopy(int id)
    {
        var endo = await _db.Endoscopies
            .Include(e => e.BiopsyBottles)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (endo == null)
            return NotFound();

        if (endo.BiopsyBottles.Count != 0)
        {
            return Conflict("Cannot delete endoscopy while biopsy bottles exist. Delete or reassign the bottles first.");
        }

        _db.Endoscopies.Remove(endo);
        await _db.SaveChangesAsync();

        return NoContent();
    }
    // POST /api/endoscopies/{id}/media
    [HttpPost("{id:int}/media")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult<MediaUploadResultDto>> UploadEndoscopyMedia(
        int id,
        IFormFile file,
        [FromForm] string? title,
        CancellationToken cancellationToken)
    {
        var endoExists = await _db.Endoscopies.AnyAsync(e => e.Id == id, cancellationToken);
        if (!endoExists)
            return NotFound("Endoscopy not found.");

        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var (relativePath, storedFileName, contentType) =
            await _storage.SaveAsync(file, cancellationToken);

        var media = new EndoscopyMedia
        {
            EndoscopyId = id,
            FileName = storedFileName,
            ContentType = contentType,
            Title = string.IsNullOrWhiteSpace(title) ? file.FileName : title,
            PseudoLink = relativePath,
            FilePath = relativePath,
            ReceivedAtUtc = DateTime.UtcNow
        };

        _db.EndoscopyMedias.Add(media);
        await _db.SaveChangesAsync(cancellationToken);

        var result = new MediaUploadResultDto
        {
            Id = media.Id,
            Title = media.Title,
            FileName = media.FileName,
            ContentType = media.ContentType,
            PseudoLink = media.PseudoLink
        };

        return CreatedAtAction(nameof(GetEndoscopy), new { id }, result);
    }
    // GET /api/Endoscopies/{id}/media
    [HttpGet("{id:int}/media")]
    public async Task<ActionResult<IEnumerable<MediaFileDto>>> GetEndoscopyMedia(int id)
    {
        // Query the derived EndoscopyMedia type (it has EndoscopyId)
        var media = await _db.Set<EndoscopyMedia>()
            .Where(m => m.EndoscopyId == id && !m.IsDeleted)
            .ToListAsync();

        var dto = _mapper.Map<List<MediaFileDto>>(media);
        return Ok(dto);
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EndoscopyListItemDto>>> Get(
    [FromQuery] int? patientId = null,
    [FromQuery] DateTime? fromUtc = null,
    [FromQuery] DateTime? toUtc = null,
    [FromQuery] EndoscopyType? type = null,
    [FromQuery] bool? urgent = null)
    {
        var query = _db.Endoscopies
            .AsNoTracking()
            .Include(e => e.Patient)
            .Include(e => e.BiopsyBottles)
            .Include(e => e.Report)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(e => e.PatientId == patientId.Value);

        if (fromUtc.HasValue)
            query = query.Where(e => e.PerformedAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(e => e.PerformedAtUtc <= toUtc.Value);

        if (type.HasValue)
            query = query.Where(e => e.Type == type.Value);

        if (urgent.HasValue)
            query = query.Where(e => e.IsUrgent == urgent.Value);

        var endoscopies = await query
            .OrderByDescending(e => e.PerformedAtUtc)
            .ThenBy(e => e.Id)
            .ToListAsync();

        var dto = _mapper.Map<List<EndoscopyListItemDto>>(endoscopies);
        return Ok(dto);
    }
}
