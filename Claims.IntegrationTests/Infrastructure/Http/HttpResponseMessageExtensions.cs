using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Claims.IntegrationTests.Infrastructure;

public static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, TestContext.Current.CancellationToken);
        return result ?? throw new InvalidOperationException("Response body was empty.");
    }
}
