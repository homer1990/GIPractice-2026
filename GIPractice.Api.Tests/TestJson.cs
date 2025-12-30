using System.Text.Json;
using GIPractice.Contracts.Common;

using System.Text.Json;

namespace GIPractice.Api.Tests;

/// <summary>
/// Central Json options for API tests.
/// 
/// Important: the API serializes StrongIntIds (e.g. PatientId) as numbers.
/// These options enable reading those ids back into record structs.
/// </summary>
public static class TestJson
{
    public static readonly JsonSerializerOptions Options = Create();

    private static JsonSerializerOptions Create()
    {
        var opt = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        opt.Converters.Add(new StrongIntIdJsonConverterFactory());

        return opt;
    }
}
