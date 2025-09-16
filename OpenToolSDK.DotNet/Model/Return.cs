using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.Model;

public record Return(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("schema")] Schema? Schema
)
{
    public static Return? FromJson(string json) => JsonSerializer.Deserialize<Return>(json);

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });
}