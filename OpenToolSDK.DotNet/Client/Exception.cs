using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.Client;

public class ResponseNullException(int code) : Exception
{
    [JsonPropertyName("code")]
    public int Code { get; } = code;

    [JsonPropertyName("message")]
    public override string Message => "Response is null";

    public string ToJson() => JsonSerializer.Serialize(this);
}

public class ErrorNullException(int code) : Exception
{
    [JsonPropertyName("code")]
    public int Code { get; } = code;

    [JsonPropertyName("message")]
    public override string Message => "Error is null";

    public string ToJson() => JsonSerializer.Serialize(this);
}

public class OpenToolServerUnauthorizedException : Exception
{
    [JsonPropertyName("code")]
    public int Code => 401;

    [JsonPropertyName("message")]
    public override string Message => "Please check API Key is VALID or NOT";

    public string ToJson() => JsonSerializer.Serialize(this);
}

public class OpenToolServerNoAccessException : Exception
{
    [JsonPropertyName("code")]
    public int Code => 404;

    [JsonPropertyName("message")]
    public override string Message => "Please check OpenTool Server is RUNNING or NOT";

    public string ToJson() => JsonSerializer.Serialize(this);
}

public class OpenToolServerCallException(string message) : Exception
{
    [JsonPropertyName("message")]
    public override string Message { get; } = message;

    public string ToJson() => JsonSerializer.Serialize(this);
}