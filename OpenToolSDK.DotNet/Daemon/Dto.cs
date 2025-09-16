using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.Daemon;

public record RegisterInfo(
    [property: JsonPropertyName("file")] string File,
    [property: JsonPropertyName("host")] string Host,
    [property: JsonPropertyName("port")] int Port,
    [property: JsonPropertyName("prefix")] string Prefix,
    [property: JsonPropertyName("apiKeys")] List<string>? ApiKeys,
    [property: JsonPropertyName("pid")] int Pid
)
{
    public static RegisterInfo? FromJson(string json) => JsonSerializer.Deserialize<RegisterInfo>(json);

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}

public record RegisterResult(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("error")] string? Error = null
)
{
    public static RegisterResult? FromJson(string json) => JsonSerializer.Deserialize<RegisterResult>(json);

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}