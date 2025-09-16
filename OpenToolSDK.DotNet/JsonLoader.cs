using System.Text.Json;
using System.Text.Json.Nodes;
using OpenToolSDK.DotNet.Model;

namespace OpenToolSDK.DotNet;

public class OpenToolJsonLoader
{
    public JsonObject? SchemasJsonRaw { get; private set; }

    public OpenTool Load(string jsonString)
    {
        try
        {
            var jObject = JsonNode.Parse(jsonString)?.AsObject();
            if (jObject is null)
                throw new JsonException("Invalid JSON: root is null or not an object.");

            if (jObject["schemas"] is JsonObject schemasToken)
            {
                try
                {
                    SchemasJsonRaw = schemasToken;
                    SchemasSingleton.InitInstance(schemasToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Failed to parse schemas: {ex.Message}");
                }
            }

            var model = OpenTool.FromJson(jsonString);
            if (model is null)
                throw new JsonException("Failed to deserialize OpenTool.");

            return model;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[Error] Failed to deserialize OpenTool: {ex.Message}");
            throw;
        }
    }

    public OpenTool LoadFromFile(string jsonPath)
    {
        var jsonString = File.ReadAllText(jsonPath);
        return Load(jsonString);
    }
}