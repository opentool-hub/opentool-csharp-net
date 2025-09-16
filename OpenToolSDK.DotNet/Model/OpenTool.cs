using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.Model;

public class OpenTool
{
    [JsonPropertyName("opentool")]
    public string? OpenToolVersion { get; set; }

    [JsonPropertyName("info")]
    public Info? Info { get; set; }

    [JsonPropertyName("server")]
    public Server? Server { get; set; }

    [JsonPropertyName("functions")]
    public List<Function>? Functions { get; set; }

    [JsonPropertyName("schemas")]
    public Dictionary<string, Schema>? Schemas { get; set; }

    public static OpenTool? FromJson(string json)
    {
        var model = JsonSerializer.Deserialize<OpenTool>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (model?.Schemas is not null)
        {
            var schemasJson = JsonSerializer.SerializeToNode(model.Schemas);
            if (schemasJson is JsonObject obj)
            {
                SchemasSingleton.InitInstance(obj);
            }
        }

        return model;
    }

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });
}