using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.Model;

public class Server
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    public static Server? FromJson(string json) => JsonSerializer.Deserialize<Server>(json);

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });
}