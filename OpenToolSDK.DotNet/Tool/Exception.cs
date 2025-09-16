using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.Tool;

/// <summary>
/// Function not supported by this Tool.
/// </summary>
public class FunctionNotSupportedException(string functionName)
    : Exception($"Function Not Supported: {functionName}")
{
    public int Code => 405;

    [JsonPropertyName("message")]
    public override string Message { get; } = $"Function Not Supported: {functionName}";

    public string ToJson() => JsonSerializer.Serialize(this);
}

/// <summary>
/// Invalid arguments passed to a function call.
/// </summary>
public class InvalidArgumentsException(Dictionary<string, object> arguments)
    : Exception($"Invalid Arguments: {JsonSerializer.Serialize(arguments)}")
{
    public int Code => 400;

    [JsonPropertyName("message")]
    public override string Message { get; } = $"Invalid Arguments: {JsonSerializer.Serialize(arguments)}";

    public string ToJson() => JsonSerializer.Serialize(this);
}

/// <summary>
/// Tool execution encountered a break-level error; clients should stop retrying.
/// </summary>
public class ToolBreakException(string? message = null) : Exception(message)
{
    public int Code => 500;

    [JsonPropertyName("message")]
    public override string Message { get; } = message ?? "Tool break exception";

    public string ToJson() => JsonSerializer.Serialize(this);
}

/// <summary>
/// JSON parser not implemented.
/// </summary>
public class JsonParserException() : Exception("Json Parser NOT implement")
{
    public int Code => 404;

    [JsonPropertyName("message")]
    public override string Message => "Json Parser NOT implement";

    public string ToJson() => JsonSerializer.Serialize(this);
}