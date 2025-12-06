using Microsoft.AspNetCore.Http;

namespace GIPractice.Api.Services;

public interface IMediaStorageService
{
    /// <summary>
    /// Stores the file on disk and returns (relativePath, fileName, contentType).
    /// relativePath is what we put into MediaFile.PseudoLink (e.g. /media/2025/12/05/guid.docx).
    /// </summary>
    Task<(string relativePath, string storedFileName, string contentType)> SaveAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);
}
