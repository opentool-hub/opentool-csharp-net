using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenToolSDK.DotNet.LLM;
using OpenToolSDK.DotNet.Model;

namespace OpenToolSDK.DotNet.Client;

public interface IClient
{
    Task<Version> Version();
    Task<ToolReturn> Call(FunctionCall functionCall);
    Task<OpenTool?> Load();
}

public class OpenToolClient : IClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public OpenToolClient(
        bool isSSL = false,
        string host = "localhost",
        int port = Constants.DEFAULT_PORT,
        string? apiKey = null)
    {
        string protocol = isSSL ? "https" : "http";
        _baseUrl = $"{protocol}://{host}:{port}{Constants.DEFAULT_PREFIX}/";

        _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }
    }

    public async Task<Version> Version()
    {
        try
        {
            var response = await _httpClient.GetAsync("version");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new OpenToolServerUnauthorizedException();

            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Version>(body, JsonOptions)!;
        }
        catch (HttpRequestException)
        {
            throw new OpenToolServerNoAccessException();
        }
    }

    public async Task<ToolReturn> Call(FunctionCall functionCall)
    {
        var result = await CallJsonRpcHttp(functionCall.Id, functionCall.Name, functionCall.Arguments);
        return new ToolReturn(functionCall.Id, result);
    }

    private async Task<Dictionary<string, object>> CallJsonRpcHttp(string id, string method, Dictionary<string, object> parameters)
    {
        var body = new JsonRpcHttpRequestBody
        {
            Id = id,
            Method = method,
            Params = parameters
        };

        try
        {
            string json = JsonSerializer.Serialize(body, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("call", content);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new OpenToolServerUnauthorizedException();

            string responseBody = await response.Content.ReadAsStringAsync();
            var resultObj = JsonSerializer.Deserialize<JsonRpcHttpResponseBody>(responseBody, JsonOptions)!;

            if (resultObj.Error != null)
                throw new OpenToolServerCallException(resultObj.Error.Message);

            return resultObj.Result!;
        }
        catch (HttpRequestException)
        {
            throw new OpenToolServerNoAccessException();
        }
    }

    public async Task<OpenTool?> Load()
    {
        try
        {
            var response = await _httpClient.GetAsync("load");
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OpenTool>(content, JsonOptions);
        }
        catch
        {
            return null;
        }
    }
}