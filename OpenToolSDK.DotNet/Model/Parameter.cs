using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.Model;

public class Parameter
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("schema")]
    public Schema? Schema { get; set; }

    [JsonPropertyName("required")]
    public bool RequiredFlag { get; set; }

    public static Parameter? FromJson(string json) => JsonSerializer.Deserialize<Parameter>(json);

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });
}