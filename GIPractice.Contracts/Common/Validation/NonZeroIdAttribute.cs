using System.ComponentModel.DataAnnotations;

namespace GIPractice.Contracts.Common.Validation;

/// <summary>
/// Validates that a "strong id" (record struct with an int Value property) is > 0.
/// Supports int/long directly. Null is treated as valid (use [Required] if you need non-null).
/// </summary>
public sealed class NonZeroIdAttribute : ValidationAttribute
{
    public NonZeroIdAttribute() : base("The field must be a non-zero id.") { }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        // unwrap Nullable<T>
        var type = value.GetType();
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var hasValue = (bool?)type.GetProperty("HasValue")?.GetValue(value);
            if (hasValue != true)
                return true;

            value = type.GetProperty("Value")?.GetValue(value);
            if (value is null)
                return true;

            type = value.GetType();
        }

        if (value is int i) return i > 0;
        if (value is long l) return l > 0;
        if (value is short s) return s > 0;
        if (value is uint ui) return ui > 0;
        if (value is ulong ul) return ul > 0;

        var vprop = type.GetProperty("Value");
        if (vprop is null)
            return true;

        var inner = vprop.GetValue(value);
        return inner switch
        {
            int iv => iv > 0,
            long lv => lv > 0,
            short sv => sv > 0,
            uint uiv => uiv > 0,
            ulong ulv => ulv > 0,
            _ => true
        };
    }
}
