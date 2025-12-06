using GIPractice.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GIPractice.Api.Services;

public class FileSystemMediaStorageService : IMediaStorageService
{
    private readonly string _rootPath;
    private readonly string _requestPath;

    public FileSystemMediaStorageService(IOptions<MediaStorageOptions> options, IWebHostEnvironment env)
    {
        var opts = options.Value;

        // If RootPath is relative, make it under content root
        _rootPath = Path.IsPathRooted(opts.RootPath)
            ? opts.RootPath
            : Path.Combine(env.ContentRootPath, opts.RootPath);

        _requestPath = opts.RequestPath?.TrimEnd('/') ?? "/media";
    }

    public async Task<(string relativePath, string storedFileName, string contentType)> SaveAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.", nameof(file));

        var now = DateTime.UtcNow;
        var dateFolder = Path.Combine(
            now.Year.ToString("D4"),
            now.Month.ToString("D2"),
            now.Day.ToString("D2"));

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            // for safety: docx/jpg/pdf detection could be added, but for now keep original no-ext as is
            extension = "";
        }

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var targetDir = Path.Combine(_rootPath, dateFolder);

        Directory.CreateDirectory(targetDir);

        var fullPath = Path.Combine(targetDir, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        // e.g. /media/2025/12/05/guid.docx
        var relativePath = $"{_requestPath}/{dateFolder.Replace('\\', '/')}/{fileName}";

        return (relativePath, fileName, file.ContentType);
    }
}
