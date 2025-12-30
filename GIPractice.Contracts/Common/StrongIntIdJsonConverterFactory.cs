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
        // Handle nullable wrappers
        var t = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;

        // Accept types that look like: readonly record struct XxxId(int Value)
        // i.e. have:
        // - public ctor(int)
        // - public int Value { get; }
        var ctor = t.GetConstructor(new[] { typeof(int) });
        if (ctor is null)
            return false;

        var valueProp = t.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
        return valueProp is { CanRead: true } && valueProp.PropertyType == typeof(int);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var underlying = Nullable.GetUnderlyingType(typeToConvert);

        if (underlying is null)
        {
            return Cache.GetOrAdd(typeToConvert, static t =>
            {
                var convType = typeof(StrongIntIdJsonConverter<>).MakeGenericType(t);
                return (JsonConverter)Activator.CreateInstance(convType)!;
            });
        }

        // Nullable<TId>: build inner converter for TId, then wrap it.
        var inner = Cache.GetOrAdd(underlying, static t =>
        {
            var convType = typeof(StrongIntIdJsonConverter<>).MakeGenericType(t);
            return (JsonConverter)Activator.CreateInstance(convType)!;
        });

        var wrapperType = typeof(NullableStrongIntIdJsonConverter<>).MakeGenericType(underlying);
        return (JsonConverter)Activator.CreateInstance(wrapperType, inner)!;
    }

    private sealed class StrongIntIdJsonConverter<TId> : JsonConverter<TId>
    {
        private static readonly ConstructorInfo Ctor =
            typeof(TId).GetConstructor(new[] { typeof(int) })!;

        private static readonly PropertyInfo ValueProp =
            typeof(TId).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance)!;

        public override TId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
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

    private sealed class NullableStrongIntIdJsonConverter<TId> : JsonConverter<TId?>
        where TId : struct
    {
        private readonly JsonConverter<TId> _inner;

        public NullableStrongIntIdJsonConverter(JsonConverter inner)
            => _inner = (JsonConverter<TId>)inner;

        public override TId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            return _inner.Read(ref reader, typeof(TId), options);
        }

        public override void Write(Utf8JsonWriter writer, TId? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            _inner.Write(writer, value.Value, options);
        }
    }
}
