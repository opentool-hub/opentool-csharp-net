using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OpenToolSDK.DotNet.Daemon;

public class DaemonClient
{
    private const int DaemonDefaultPort = 19627;
    private const string DaemonDefaultPrefix = "/opentool-daemon";

    private readonly string _protocol = "http";
    private readonly string _host = "localhost";
    private readonly int _port;
    private readonly string _prefix;
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public DaemonClient(int? port = null)
    {
        _port = port is > 0 ? port.Value : DaemonDefaultPort;
        _prefix = DaemonDefaultPrefix;

        string baseUrl = $"{_protocol}://{_host}:{_port}{_prefix}/";
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<RegisterResult> Register(RegisterInfo registerInfo)
    {
        try
        {
            string json = JsonSerializer.Serialize(registerInfo, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("register", content);
            response.EnsureSuccessStatusCode();

            string resultJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RegisterResult>(resultJson, JsonOptions)!;
        }
        catch (HttpRequestException e)
        {
            return new RegisterResult(Id: "-1", Error: e.Message);
        }
    }
}