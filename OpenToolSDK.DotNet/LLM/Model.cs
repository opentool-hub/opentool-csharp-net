using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.LLM;

public record FunctionCall(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("arguments")] Dictionary<string, object> Arguments
)
{
    public static FunctionCall? FromJson(string json) => JsonSerializer.Deserialize<FunctionCall>(json);

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}

public record ToolReturn(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("result")] Dictionary<string, object> Result
)
{
    public static ToolReturn? FromJson(string json) => JsonSerializer.Deserialize<ToolReturn>(json);

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}