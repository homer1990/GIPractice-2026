using GIPractice.Api.Models;
using GIPractice.Api.Services;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController(AppDbContext db, IMediaStorageService storage) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMediaStorageService _storage = storage;

    // POST /api/media
    // multipart/form-data: file, optional title
    [HttpPost]
    [RequestSizeLimit(100_000_000)] // ~100MB, adjust as needed
    public async Task<ActionResult<MediaUploadResultDto>> Upload(
        IFormFile file,
        [FromForm] string? title,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var (relativePath, storedFileName, contentType) =
            await _storage.SaveAsync(file, cancellationToken);

        var media = new MediaFile
        {
            FileName = storedFileName,
            ContentType = contentType,
            Title = string.IsNullOrWhiteSpace(title) ? file.FileName : title,
            PseudoLink = relativePath,
            FilePath = relativePath, // you can store full physical path if you prefer
            ReceivedAtUtc = DateTime.UtcNow
        };

        _db.MediaFiles.Add(media);
        await _db.SaveChangesAsync(cancellationToken);

        var result = new MediaUploadResultDto
        {
            Id = media.Id,
            Title = media.Title,
            FileName = media.FileName,
            ContentType = media.ContentType,
            PseudoLink = media.PseudoLink
        };

        return CreatedAtAction(nameof(Get), new { id = media.Id }, result);
    }

    // GET /api/media/{id}
    // Serve the file back to the client
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var media = await _db.MediaFiles
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (media == null)
            return NotFound();

        // If FilePath stores relative path under wwwroot, map it to physical path
        // e.g. /media/2025/12/05/file.docx -> wwwroot/media/2025/12/05/file.docx
        var env = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var filePath = media.FilePath;
        if (!Path.IsPathRooted(filePath))
        {
            // Strip leading '/' if present
            var trimmed = filePath.TrimStart('/');
            filePath = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), trimmed);
        }

        if (!System.IO.File.Exists(filePath))
            return NotFound("File missing on disk.");

        var stream = System.IO.File.OpenRead(filePath);
        return File(stream, media.ContentType, media.FileName);
    }
}
