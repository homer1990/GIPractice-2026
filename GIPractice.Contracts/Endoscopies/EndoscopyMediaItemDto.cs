using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopyMediaItemDto(
    EndoscopyMediaId Id,
    EndoscopyId EndoscopyId,
    DateTime CapturedUtc,
    string MediaType,      // "image/jpeg", "video/mp4"
    string FileName,
    long LengthBytes);
