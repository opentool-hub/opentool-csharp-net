using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.Model;

public class Info
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    public static Info? FromJson(string json) => JsonSerializer.Deserialize<Info>(json);

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });
}