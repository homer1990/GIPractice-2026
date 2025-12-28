using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchRowDto(
    EndoscopyId EndoscopyId,
    PatientId PatientId,

    string PatientFirstName,
    string PatientLastName,

    string EndoscopyLabel,
    int BottleCount,
    decimal CalculatedPrice,
    bool IsUrgent);
