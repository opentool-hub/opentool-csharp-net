﻿using OpenToolSDK.DotNet.Client;
using OpenToolSDK.DotNet.LLM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenToolSDK.Example;

public class ClientMain
{
    public static async Task Run()
    {
        var client = new OpenToolClient(apiKey: "bb31b6a6-1fda-4214-8cd6-b1403842070c");

        try
        {
            var version = await client.Version();
            Console.WriteLine("=== [Version] ===");
            Console.WriteLine(version.ToJson());

            var functionCall = new FunctionCall(
                "callId-0",
                "count",
                new Dictionary<string, object>()
            );

            var result = await client.Call(functionCall);
            Console.WriteLine("\n=== [Call Result] ===");
            Console.WriteLine(result.ToJson());

            var openTool = await client.Load();
            Console.WriteLine("\n=== [OpenTool] ===");
            Console.WriteLine(openTool?.ToJson() ?? "No OpenTool loaded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("=== [Error] ===");
            Console.WriteLine(ex.ToString());
        }
    }
}