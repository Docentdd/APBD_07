using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Tutorial3_Task.RestAPI.Controllers;

[ApiController]
[Route("api/devices")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceManager _deviceManager;

    public DeviceController(IDeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
    }
    [HttpGet]
    public IResult getAllDevices()
    {
        var devices = _deviceManager.getAllDevices();
        return Results.Ok(devices);
    }
    [HttpGet("{id}")]
    public IResult getDeviceById(string id)
    {
        var device = _deviceManager.getDeviceById(id);
        if (device == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(device);
    }
    [HttpDelete("{id}")]
    public IResult DeleteDevice(string id)
    {
        var existingDevice = _deviceManager.getDeviceById(id);
        if (existingDevice == null)
        {
            return Results.NotFound();
        }
        _deviceManager.DeleteDevice(id);
        return Results.NoContent();
    }
    [HttpPost]
    public IResult AddDevice([FromBody] JsonElement payload)
    {
        if (!payload.TryGetProperty("jsonString", out JsonElement jsonStringElement) || jsonStringElement.ValueKind != JsonValueKind.String)
        {
            return Results.BadRequest(new { message = "The jsonString field is required and must be a string." });
        }

        string jsonString = jsonStringElement.GetString();

        try
        {
            _deviceManager.AddDevice(jsonString);
            return Results.Ok(new { message = "Device added successfully." });
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }
    
    [HttpPut]
    public IResult UpdateDevice([FromBody] JsonElement payload)
    {
        if (!payload.TryGetProperty("jsonString", out JsonElement jsonStringElement) || jsonStringElement.ValueKind != JsonValueKind.String)
        {
            return Results.BadRequest(new { message = "The jsonString field is required and must be a string." });
        }

        string jsonString = jsonStringElement.GetString();

        try
        {
            _deviceManager.UpdateDevice(jsonString);
            return Results.Ok(new { message = "Device updated successfully." });
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }
}