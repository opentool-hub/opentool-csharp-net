# OpenTool SDK for .NET

English | [中文](README-zh_CN.md)

The OpenTool SDK for .NET supports Tool Server and Client communication over the JSON-RPC protocol, implementing the core capabilities defined in the OpenTool specification.

## Features

* OpenTool JSON parser
* JSON-RPC over HTTP Client and Server
* API Key authorization middleware
* Interoperability with OpenTool Client/Server implemented in C# .NET

Add the following dependency in your NuGet configuration:

```
OpenToolSDK.DotNet
```

## Examples

1. Implementing a sample Tool:

   ```csharp
   public class MockTool : ITool
   {
       private readonly MockUtil _mockUtil = new();
   
      public Task<Dictionary<string, object>> Call(string name, Dictionary<string, object> arguments)
      {
         switch (name)
         {
          case "count":
              return Task.FromResult(new Dictionary<string, object>
              {
                  { "count", _mockUtil.Count() }
              });
         
          default:
              throw new FunctionNotSupportedException(name);
         }
      }
   
      public async ValueTask<OpenTool?> Load()
      {
         string assemblyPath = Path.GetDirectoryName(typeof(MockTool).Assembly.Location)!;
         string jsonPath = Path.Combine(assemblyPath, "mock_tool.json");
         
         var loader = new OpenToolJsonLoader();
         var openTool = loader.LoadFromFile(jsonPath);
         return await Task.FromResult(openTool);
      }
   }
   ```

2. Starting the OpenTool Server:

   ```csharp
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
   ```

## Usage Notes

1. The default port is `9627`. You can change it via the `OpenToolServer` constructor.
2. A new Tool must implement the `ITool` interface, specifically the `Call` method. It is recommended to implement `Load` to load OpenTool JSON descriptions.
3. It is recommended to use [OpenTool specification JSON files](https://github.com/opentool-hub/opentool-spec) to define tools. Constructing `OpenTool` objects programmatically is also supported.
