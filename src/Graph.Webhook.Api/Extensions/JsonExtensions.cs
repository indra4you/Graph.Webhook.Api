using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Graph.Webhook.Api;

internal static class JsonExtensions
{
    internal static async Task<string> ToJsonString(
        this Stream stream,
        ILogger? logger = null
    )
    {
        using var reader = new StreamReader(stream);
        var jsonString = await reader.ReadToEndAsync();
        logger?.LogInformation(jsonString);
        return jsonString;
    }

    internal static T? ToJson<T>(
        this string jsonString
    )
        => JsonSerializer.Deserialize<T>(jsonString);

    internal static async Task<T?> ToJson<T>(
        this Stream stream,
        ILogger? logger = null
    )
    {
        var jsonString = await stream.ToJsonString(logger);
        return jsonString.ToJson<T>();
    }
}