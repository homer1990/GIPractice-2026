using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;
using Xunit;

public sealed class ValidationTests
{
    [Fact]
    public void NonZeroIdAttribute_ShouldFail_OnZero()
    {
        var attr = new NonZeroIdAttribute();

        var r = attr.GetValidationResult(new PatientId(0), new ValidationContext(new object()));

        r.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void NotDefaultAttribute_ShouldFail_OnDefaultDateTime()
    {
        var attr = new NotDefaultAttribute();

        var r = attr.GetValidationResult(default(DateTime), new ValidationContext(new object()));

        r.Should().NotBe(ValidationResult.Success);
    }
}
