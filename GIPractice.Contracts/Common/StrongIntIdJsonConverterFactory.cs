using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GIPractice.Contracts.Common;

public sealed class StrongIntIdJsonConverterFactory : JsonConverterFactory
{
    private static readonly ConcurrentDictionary<Type, JsonConverter> Cache = new();

    public override bool CanConvert(Type typeToConvert)
    {
        // Accept types that look like: readonly record struct XxxId(int Value)
        // i.e. have:
        // - public ctor(int)
        // - public int Value { get; }
        var ctor = typeToConvert.GetConstructor(new[] { typeof(int) });
        if (ctor is null)
            return false;

        var valueProp = typeToConvert.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
        return valueProp is { CanRead: true } && valueProp.PropertyType == typeof(int);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        Cache.GetOrAdd(typeToConvert, static t =>
        {
            var convType = typeof(StrongIntIdJsonConverter<>).MakeGenericType(t);
            return (JsonConverter)Activator.CreateInstance(convType)!;
        });

    private sealed class StrongIntIdJsonConverter<TId> : JsonConverter<TId>
    {
        private static readonly ConstructorInfo Ctor =
            typeof(TId).GetConstructor(new[] { typeof(int) })!;

        private static readonly PropertyInfo ValueProp =
            typeof(TId).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance)!;

        public override TId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Allow both number and string inputs (string is handy if some clients send "123")
            int value = reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetInt32(),
                JsonTokenType.String => int.Parse(reader.GetString() ?? "", System.Globalization.CultureInfo.InvariantCulture),
                _ => throw new JsonException($"Cannot parse {typeof(TId).Name} from token {reader.TokenType}.")
            };

            return (TId)Ctor.Invoke(new object[] { value });
        }

        public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
        {
            var raw = (int)ValueProp.GetValue(value)!;
            writer.WriteNumberValue(raw);
        }
    }
}
