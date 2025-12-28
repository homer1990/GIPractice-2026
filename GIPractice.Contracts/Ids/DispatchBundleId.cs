using System;

namespace GIPractice.Contracts.Ids;

[Obsolete("Use BiopsyDispatchBundleId.")]
public readonly record struct DispatchBundleId(int Value);