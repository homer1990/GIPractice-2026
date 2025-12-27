using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Dispatch;

public sealed record BiopsyDispatchRowDto(
    EndoscopyId EndoscopyId,
    PatientId PatientId,
    string PatientFirstName,
    string PatientLastName,
    string EndoscopyLabel,     // "Double" / "Γαστρο" etc.
    int BottleCount,
    decimal CalculatedPrice,
    bool IsUrgent);
