namespace GIPractice.Contracts.Localization;

public sealed record TranslationDto(
    string Key,
    string Culture,        // e.g. "en-US", "el-GR"
    string Value,
    string? Module,        // optional: "Patients", "Scheduling", etc.
    string? Notes,
    byte[]? RowVersion);
