using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenToolSDK.DotNet.Daemon;
using OpenToolSDK.DotNet.Tool;
using System.Net;

namespace OpenToolSDK.DotNet.Server;

public interface IServer
{
    Task Start();
    Task Stop();
}

public class OpenToolServer : IServer
{
    private readonly ITool _tool;
    private readonly string _version;
    private readonly string _ip;
    private readonly int _port;
    private readonly string _prefix;
    private readonly List<string> _apiKeys;

    private IHost? _host;

    public OpenToolServer(
        ITool tool,
        string version,
        string ip = "127.0.0.1",
        int port = Constants.DEFAULT_PORT,
        string prefix = Constants.DEFAULT_PREFIX,
        List<string>? apiKeys = null)
    {
        _tool = tool;
        _version = version;
        _ip = ip;
        _port = port;
        _prefix = prefix.StartsWith("/") ? prefix : "/" + prefix;
        _apiKeys = apiKeys ?? new List<string>();
    }

    public async Task Start()
    {
        ToolRegistry.ToolInstance = _tool;
        ToolRegistry.Version = _version;

        _host = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning); // 只输出 Warning 以上
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel()
                    .UseUrls($"http://{_ip}:{_port}")
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton(_tool);
                        services.AddControllers();

                        if (_apiKeys.Count > 0)
                        {
                            services.AddSingleton<IEnumerable<string>>(_apiKeys);
                        }
                    })
                    .Configure(app =>
                    {
                        if (_apiKeys.Count > 0)
                        {
                            app.UseMiddleware<AuthorizationMiddleware>(_apiKeys);
                        }

                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
            })
            .Build();

        await _host.StartAsync();

        Console.WriteLine($"[OpenToolSDK] Server started: http://{_ip}:{_port}{_prefix}");

        await RegisterToDaemon();
    }

    public async Task Stop()
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
            Console.WriteLine("[OpenToolSDK] Server stopped.");
        }
    }

    private async Task RegisterToDaemon()
    {
        var registerInfo = new RegisterInfo(
            File: System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "unknown",
            Host: IPAddress.Loopback.ToString(),
            Port: _port,
            Prefix: _prefix,
            ApiKeys: _apiKeys,
            Pid: Environment.ProcessId
        );

        var client = new DaemonClient();
        var result = await client.Register(registerInfo);

        if (!string.IsNullOrEmpty(result.Error))
        {
            Console.WriteLine($"[OpenToolSDK] WARNING: Register to daemon failed. ({result.Error})");
            Console.WriteLine("[OpenToolSDK] Tool running in SOLO mode.");
        }
        else
        {
            Console.WriteLine($"[OpenToolSDK] Registered to daemon successfully, id: {result.Id}, pid: {registerInfo.Pid}");
        }
    }
}