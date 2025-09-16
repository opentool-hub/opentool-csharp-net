using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenToolSDK.DotNet.Model;

public static class SchemaType
{
    public const string BOOLEAN = "boolean";
    public const string INTEGER = "integer";
    public const string NUMBER = "number";
    public const string STRING = "string";
    public const string ARRAY = "array";
    public const string OBJECT = "object";
}

public class Schema
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, Schema>? Properties { get; set; }

    [JsonPropertyName("items")]
    public Schema? Items { get; set; }

    [JsonPropertyName("enum")]
    public List<object>? Enum { get; set; }

    [JsonPropertyName("required")]
    public List<string>? Required { get; set; }

    public static Schema? FromJson(JsonNode? json)
    {
        if (json is null) return null;

        // handle $ref
        if (json["$ref"] is JsonValue refNode)
        {
            string? refStr = refNode.ToString();
            if (refStr is not null)
            {
                var schemaObj = FromRef(refStr);
                schemaObj.ValidateEnumConsistency();
                return schemaObj;
            }
        }

        var schema = JsonSerializer.Deserialize<Schema>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        schema?.ValidateEnumConsistency();
        return schema;
    }

    public JsonNode ToJson()
    {
        var node = new JsonObject
        {
            ["type"] = Type
        };

        if (!string.IsNullOrEmpty(Description))
            node["description"] = Description;

        if (Properties is not null)
        {
            var props = new JsonObject();
            foreach (var kv in Properties)
            {
                props[kv.Key] = kv.Value?.ToJson();
            }
            node["properties"] = props;
        }

        if (Items is not null)
            node["items"] = Items.ToJson();

        if (Enum is not null)
            node["enum"] = JsonSerializer.SerializeToNode(Enum);

        if (Required is not null)
            node["required"] = JsonSerializer.SerializeToNode(Required);

        return node;
    }

    private static Schema FromRef(string refStr)
    {
        var parts = refStr.Split('/');
        if (parts.Length == 3 && parts[0] == "#" && parts[1] == "schemas")
        {
            string refName = parts[2];
            var schemas = SchemasSingleton.GetInstance();
            if (schemas.TryGetValue(refName, out var schema))
            {
                return schema;
            }
            else
            {
                throw new FormatException($"#ref not found: {refStr}");
            }
        }
        else
        {
            throw new FormatException($"#ref format exception: {refStr}");
        }
    }

    private void ValidateEnumConsistency()
    {
        if (Enum is null || Enum.Count == 0)
            return;

        for (int i = 0; i < Enum.Count; i++)
        {
            var value = Enum[i];
            if (!IsValueConsistentWithType(value))
            {
                throw new FormatException($"Enum value at index {i} (\"{value}\") does not match schema type \"{Type}\".");
            }
        }
    }

    private bool IsValueConsistentWithType(object? value) =>
        Type switch
        {
            SchemaType.STRING => value is string || value is null,
            SchemaType.INTEGER => value is int || value is long || value is null,
            SchemaType.NUMBER => value is float || value is double || value is decimal || value is null,
            SchemaType.BOOLEAN => value is bool || value is null,
            "null" => value is null,
            _ => true
        };
}

public static class SchemasSingleton
{
    private static readonly Dictionary<string, Schema> _refSchemas = new();

    public static void InitInstance(JsonNode schemasJson)
    {
        if (schemasJson is not JsonObject obj) return;

        foreach (var property in obj)
        {
            string schemaName = property.Key;
            if (property.Value is JsonObject schemaObj)
            {
                _refSchemas[schemaName] = Schema.FromJson(schemaObj)!;
            }
            else
            {
                Console.WriteLine($"[Warning] Schema '{schemaName}' is not a valid object.");
            }
        }
    }

    public static Dictionary<string, Schema> GetInstance() => _refSchemas;

    public static Schema? GetSchema(string name) => _refSchemas.TryGetValue(name, out var schema) ? schema : null;
}