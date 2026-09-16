namespace GIPractice.Server.Pathology;

public sealed record BiopsyPricingPolicy(
    string Code,
    decimal BasePrice,
    int IncludedContainers,
    decimal AboveIncludedSurcharge,
    decimal AdditionalContainerPrice,
    string Currency = "EUR")
{
    // billableContainerCount means the containers from ONE PathologyCase/Endoscopy
    // that are physically included in the Parcel being billed. Containers released
    // externally do not contribute to this count.
    public BiopsyChargeCalculation Calculate(
        int billableContainerCount,
        PathologyFeeWaiverReason waiverReason = PathologyFeeWaiverReason.None)
    {
        if (string.IsNullOrWhiteSpace(Code))
            throw new ArgumentException("Pricing policy code is required.", nameof(Code));
        if (BasePrice < 0) throw new ArgumentOutOfRangeException(nameof(BasePrice));
        if (IncludedContainers < 1) throw new ArgumentOutOfRangeException(nameof(IncludedContainers));
        if (AboveIncludedSurcharge < 0) throw new ArgumentOutOfRangeException(nameof(AboveIncludedSurcharge));
        if (AdditionalContainerPrice < 0) throw new ArgumentOutOfRangeException(nameof(AdditionalContainerPrice));
        if (billableContainerCount < 1) throw new ArgumentOutOfRangeException(nameof(billableContainerCount));

        var additionalContainers = Math.Max(0, billableContainerCount - IncludedContainers);
        var surcharge = additionalContainers > 0 ? AboveIncludedSurcharge : 0m;
        var additionalAmount = additionalContainers * AdditionalContainerPrice;
        var calculated = BasePrice + surcharge + additionalAmount;
        var charged = waiverReason == PathologyFeeWaiverReason.None ? calculated : 0m;

        return new BiopsyChargeCalculation(
            Code.Trim(),
            billableContainerCount,
            BasePrice,
            surcharge,
            additionalContainers,
            AdditionalContainerPrice,
            calculated,
            charged,
            Currency,
            waiverReason);
    }
}

public sealed record BiopsyChargeCalculation(
    string PricingPolicyCode,
    int BillableContainerCount,
    decimal BaseAmount,
    decimal SurchargeAmount,
    int AdditionalContainerCount,
    decimal AdditionalContainerUnitPrice,
    decimal CalculatedAmount,
    decimal ChargedAmount,
    string Currency,
    PathologyFeeWaiverReason WaiverReason)
{
    // Initial biopsy processing is billed in the Parcel whose physical membership
    // produced BillableContainerCount. Later assay charges use separate ledger entries.
    public PathologyCharge ToInitialCharge(
        Guid pathologyCaseId,
        Guid parcelId,
        DateTimeOffset createdAtUtc) =>
        new(
            Guid.CreateVersion7(),
            pathologyCaseId,
            PathologyChargeKind.InitialBiopsy,
            createdAtUtc.ToUniversalTime(),
            CalculatedAmount,
            ChargedAmount,
            Currency,
            WaiverReason,
            BillableContainerCount,
            PricingPolicyCode,
            BilledInParcelId: parcelId,
            Description: BuildDescription());

    private string BuildDescription() =>
        AdditionalContainerCount == 0
            ? $"Biopsy processing: {BillableContainerCount} parcel container(s), base {BaseAmount:0.00} {Currency}."
            : $"Biopsy processing: {BillableContainerCount} parcel container(s), base {BaseAmount:0.00} + surcharge {SurchargeAmount:0.00} + {AdditionalContainerCount} × {AdditionalContainerUnitPrice:0.00} {Currency}.";
}
