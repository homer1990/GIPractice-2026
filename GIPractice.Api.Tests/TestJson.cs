using System.Text.Json;
using System.Text.Json.Serialization;
using GIPractice.Contracts.Common;

namespace GIPractice.Api.Tests;

public static class TestJson
{
    public static readonly JsonSerializerOptions Options = Create();

    private static JsonSerializerOptions Create()
    {
        var opt = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        opt.Converters.Add(new StrongIntIdJsonConverterFactory());
        opt.Converters.Add(new JsonStringEnumConverter());
        return opt;
    }
}