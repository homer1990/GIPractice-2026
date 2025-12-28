using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace GIPractice.Api.Common;

public sealed class StrongIntIdSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var clrType = context.JsonTypeInfo.Type;

        if (!IsStrongIntId(clrType))
            return Task.CompletedTask;

        // Force the schema to be an integer (our IDs serialize as numbers).
        schema.Type = JsonSchemaType.Integer;
        schema.Format = "int32";

        // Don't touch references: in the new OpenAPI model, schema refs aren't a property on OpenApiSchema.
        return Task.CompletedTask;
    }

    private static bool IsStrongIntId(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        // Mirror StrongIntIdJsonConverterFactory's detection:
        // - public ctor(int)
        // - public int Value { get; }
        var ctor = type.GetConstructor(new[] { typeof(int) });
        if (ctor is null)
            return false;

        var valueProp = type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
        return valueProp is { CanRead: true } && valueProp.PropertyType == typeof(int);
    }
}
