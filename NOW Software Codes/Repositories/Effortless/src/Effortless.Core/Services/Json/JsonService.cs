using System.Text.Json;
using System.Text.Json.Serialization;

namespace Effortless.Core.Services.Json;

public static class JsonService
{
    public static T? Deserialize<T>(string text)
    {
        return JsonSerializer.Deserialize<T>(text);
    }

    public static string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, DefaultConfigurations());
    }

    private static JsonSerializerOptions DefaultConfigurations()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
    }
}
