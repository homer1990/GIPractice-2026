namespace GIPractice.Server.Clinical;

public enum MediaKind
{
    Image = 1,
    Video = 2
}

public enum MediaVariant
{
    Original = 1,
    Display = 2,
    Thumbnail = 3
}

public static class ClinicalMediaDefaults
{
    public const string StillDisplayFormat = "avif";
    public const string VideoCodec = "av1";
}

public sealed record ClinicalMedia(
    Guid Id,
    Guid EndoscopyId,
    MediaKind Kind,
    MediaVariant Variant,
    string StorageKey,
    string MimeType,
    string Sha256,
    int Width,
    int Height,
    DateTimeOffset CapturedAtUtc,
    string? Codec = null,
    string? Container = null,
    int? BitDepth = null,
    long? DurationMilliseconds = null,
    Guid? FindingId = null,
    Guid? DerivedFromMediaId = null,
    string? Caption = null);
