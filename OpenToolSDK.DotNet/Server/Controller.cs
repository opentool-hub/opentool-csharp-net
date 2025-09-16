using Microsoft.AspNetCore.Mvc;
using OpenToolSDK.DotNet.Tool;
using System.Net;
using System.Text.Json;
using OpenToolSDK.DotNet.Model;

namespace OpenToolSDK.DotNet.Server;

public static class ToolRegistry
{
    public static ITool ToolInstance { get; set; } = default!;
    public static string Version { get; set; } = "1.0.0";
}

[ApiController]
[Route("opentool")]
public class ServerController : ControllerBase
{
    private readonly ITool _tool;
    private readonly string _version;

    public ServerController(ITool tool)
    {
        _tool = tool;
        _version = ToolRegistry.Version;
    }

    // GET /opentool/version
    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        var versionObj = new { VersionNumber = _version };
        return Ok(versionObj);
    }

    // POST /opentool/call
    [HttpPost("call")]
    public async Task<IActionResult> Call()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();
            var body = JsonSerializer.Deserialize<JsonRpcHttpRequestBody>(json);

            if (body is null) return BadRequest("Invalid request body");

            var result = await _tool.Call(body.Method, body.Params);
            var response = new JsonRpcHttpResponseBody
            {
                Id = body.Id,
                Result = result
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            var error = new JsonRpcHttpResponseBodyError
            {
                Code = 500,
                Message = ex.ToString()
            };

            var response = new JsonRpcHttpResponseBody
            {
                Id = "",
                Result = new Dictionary<string, object>(),
                Error = error
            };

            return StatusCode((int)HttpStatusCode.InternalServerError, response);
        }
    }

    // GET /opentool/load
    [HttpGet("load")]
    public async Task<IActionResult> Load()
    {
        try
        {
            OpenTool? opentool = await _tool.Load();
            if (opentool is not null)
            {
                return Ok(JsonSerializer.Deserialize<object>(opentool.ToJson()));
            }

            var err = new JsonParserException();
            return Ok(JsonSerializer.Deserialize<object>(err.ToJson()));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.ToString() });
        }
    }
}