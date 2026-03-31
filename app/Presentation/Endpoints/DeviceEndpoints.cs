using Application.DTOs.DeviceDtos;
using Application.Interfaces;

namespace Presentation.Endpoints;

public static class DeviceEndpoints
{
    public static void MapDeviceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/devices").WithTags("Devices");
        group.MapGet("/", async (IDeviceService deviceService) =>
        {
            var devices = await deviceService.GetAllDevicesAsync();
            return Results.Ok(devices);
        });

        group.MapGet("/{id:int}", async (int id, IDeviceService deviceService) =>
        {
            try
            {
                var device = await deviceService.GetDeviceByIdAsync(id);
                return Results.Ok(device);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { ex.Message });
            }
        });

        group.MapPost("/", async (CreateDeviceRequestDto createDeviceDto, IDeviceService deviceService) =>
        {
            await deviceService.CreateDeviceAsync(createDeviceDto);
            return Results.Created($"/api/devices/", new {Message = "Device created successfully"});
        });

        group.MapPatch("/{id:int}", async (int id, UpdateDeviceRequestDto updateDeviceDto, IDeviceService deviceService) =>
        {
            try
            {
                await deviceService.UpdateDeviceAsync(id, updateDeviceDto);
                return Results.Ok(new {Message = "Device updated successfully"});
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { ex.Message });
            }
        });

        group.MapDelete("/{id:int}", async (int id, IDeviceService deviceService) =>
        {
            try
            {
                await deviceService.DeleteDeviceAsync(id);
                return Results.Ok(new {Message = "Device deleted successfully"});
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { ex.Message });
            }
        });
    }
}
