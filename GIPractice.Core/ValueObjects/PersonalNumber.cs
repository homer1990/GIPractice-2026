namespace GIPractice.Core.ValueObjects;

public sealed class PersonalIdentifierValidityException(string? value) : Exception($"Invalid Personal Number: '{value}'")
{
}

public readonly record struct PersonalNumber
{
    public string Value { get; }

    private PersonalNumber(string value) => Value = value;

    public static PersonalNumber Create(string raw)
    {
        if (!TryNormalize(raw, out var normalized))
            throw new PersonalIdentifierValidityException(raw);

        return new PersonalNumber(normalized);
    }

    public static bool TryCreate(string? raw, out PersonalNumber result)
    {
        if (TryNormalize(raw, out var normalized))
        {
            result = new PersonalNumber(normalized);
            return true;
        }

        result = default;
        return false;
    }

    private static bool TryNormalize(string? raw, out string normalized)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(raw))
            return false;

        var s = raw.Trim();

        // Adjust according to official spec; placeholder: exactly 12 digits
        if (s.Length != 12)
            return false;

        for (int i = 0; i < s.Length; i++)
        {
            if (!char.IsDigit(s[i]))
                return false;
        }

        normalized = s;
        return true;
    }

    public override string ToString() => Value;
}
