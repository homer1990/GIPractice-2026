using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyGroupDto(
    EndoscopyId EndoscopyId,
    PatientId PatientId,
    string PatientFullName,
    string EndoscopyTypeName,
    IReadOnlyList<BiopsyBottleDto> Bottles);
