using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet;

public static class Constants
{
    public const string JSONRPC_VERSION = "2.0";
    public const int DEFAULT_PORT = 9627;
    public const string DEFAULT_PREFIX = "/opentool";
}

public class Version
{
    [JsonPropertyName("version")]
    public string VersionNumber { get; set; } = string.Empty;

    public static Version? FromJson(string json) =>
        JsonSerializer.Deserialize<Version>(json);

    public string ToJson() =>
        JsonSerializer.Serialize(this);
}

public class JsonRpcHttpRequestBody
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = Constants.JSONRPC_VERSION;

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("params")]
    public Dictionary<string, object>? Params { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    public static JsonRpcHttpRequestBody? FromJson(string json) =>
        JsonSerializer.Deserialize<JsonRpcHttpRequestBody>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    public string ToJson() =>
        JsonSerializer.Serialize(this);
}

public class JsonRpcHttpResponseBody
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = Constants.JSONRPC_VERSION;

    [JsonPropertyName("result")]
    public Dictionary<string, object>? Result { get; set; }

    [JsonPropertyName("error")]
    public JsonRpcHttpResponseBodyError? Error { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    public static JsonRpcHttpResponseBody? FromJson(string json) =>
        JsonSerializer.Deserialize<JsonRpcHttpResponseBody>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    public string ToJson() =>
        JsonSerializer.Serialize(this);
}

public class JsonRpcHttpResponseBodyError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    public static JsonRpcHttpResponseBodyError? FromJson(string json) =>
        JsonSerializer.Deserialize<JsonRpcHttpResponseBodyError>(json);

    public string ToJson() =>
        JsonSerializer.Serialize(this);
}