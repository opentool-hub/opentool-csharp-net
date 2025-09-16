using OpenToolSDK.DotNet.Model;

namespace OpenToolSDK.DotNet.Tool;

public interface ITool
{
    Task<Dictionary<string, object>> Call(string name, Dictionary<string, object> arguments);

    ValueTask<OpenTool?> Load() => ValueTask.FromResult<OpenTool?>(null);
}