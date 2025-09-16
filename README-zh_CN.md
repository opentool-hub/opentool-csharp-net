# OpenTool SDK for .NET

[English](README.md) | 中文

OpenTool 的 .NET SDK，支持 JSON-RPC 协议的 Tool Server 与 Client，实现了 OpenTool 规范中的核心能力。

## 功能

* OpenTool JSON 解析器
* JSON-RPC over HTTP 的 Client 和 Server
* 支持 API Key 认证中间件
* C# .NET SDK 的 OpenTool Client/Server 的互操作

在 `NuGet` 文件中增加如下依赖：

```
OpenToolSDK.DotNet
```

## 示例

1. 实现一个 Tool 示例：

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
   
2. 启动 OpenTool Server：

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

## 使用说明

1. 默认端口为 `9627`，如需修改，可在 `OpenToolServer` 构造函数中指定。
2. 新的 Tool 需要实现 `ITool` 接口中的 `Call` 方法，推荐实现 `Load` 方法用于加载 OpenTool JSON 描述。
3. 推荐使用 [OpenTool 规范的 JSON 文件](https://github.com/opentool-hub/opentool-spec) 来定义工具信息，当然也可以通过代码构建 `OpenTool` 对象。