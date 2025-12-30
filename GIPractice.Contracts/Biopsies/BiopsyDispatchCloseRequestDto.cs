using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchCloseRequestDto(
    [param: NonZeroId] BiopsyDispatchBundleId? Id,
    byte[]? RowVersion);
