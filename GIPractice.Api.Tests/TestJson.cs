using GIPractice.Contracts.Common;

using System.Text.Json;

namespace GIPractice.Api.Tests;

public static class TestJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    static TestJson()
    {
        Options.Converters.Add(new StrongIntIdJsonConverterFactory());
    }
}
