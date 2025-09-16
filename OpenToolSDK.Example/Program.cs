using OpenToolSDK.DotNet.Client;
using OpenToolSDK.DotNet.LLM;
using OpenToolSDK.DotNet.Server;

namespace OpenToolSDK.Example;

internal static class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Launching OpenTool Server... ===");

        var tool = new MockTool();
        var server = new OpenToolServer(
            tool: tool,
            version: "1.0.0",
            apiKeys:
            [
                "6621c8a3-2110-4e6a-9d62-70ccd467e789",
                "bb31b6a6-1fda-4214-8cd6-b1403842070c"
            ]
        );

        await server.Start();

        await Task.Delay(300);

        Console.WriteLine("\n=== Running OpenTool Client... ===");

        var client = new OpenToolClient(apiKey: "bb31b6a6-1fda-4214-8cd6-b1403842070c");

        var version = await client.Version();
        Console.WriteLine($"[Client] Server version: {version.VersionNumber}");

        var call = new FunctionCall(
            Guid.NewGuid().ToString(),
            "count",
            new Dictionary<string, object>()
        );

        var result = await client.Call(call);
        Console.WriteLine($"[Client] Call result: {result.ToJson()}");

        var opentool = await client.Load();
        Console.WriteLine($"[Client] Load result: {opentool?.ToJson() ?? "null"}");

        Console.WriteLine("\nPress ENTER to stop server...");
        Console.ReadLine();

        await server.Stop();
    }
}