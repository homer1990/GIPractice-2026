using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopyTypeDto(
    EndoscopyTypeId Id,
    string Name);