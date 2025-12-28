using System.ComponentModel.DataAnnotations;

namespace GIPractice.Contracts.Common.Validation;

/// <summary>
/// Validates that a value-type isn't its default value (e.g. DateTime != default).
/// Null is treated as valid (use [Required] if you need non-null).
/// </summary>
public sealed class NotDefaultAttribute : ValidationAttribute
{
    public NotDefaultAttribute() : base("The field must not be the default value.") { }

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
        }

        return value switch
        {
            DateTime dt => dt != default,
            DateOnly d => d != default,
            TimeOnly t => t != default,
            _ => true
        };
    }
}
