using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.Model;

public class Function
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("parameters")]
    public List<Parameter>? Parameters { get; set; }

    [JsonPropertyName("return")]
    public Return? Return { get; set; }

    public static Function? FromJson(string json) => JsonSerializer.Deserialize<Function>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });
}