namespace GIPractice.Contracts.Localization;

public sealed record TranslationEntryDto(
    string Culture,   // "el-GR"
    string Key,       // "Patients.Search.FirstName"
    string Value);
