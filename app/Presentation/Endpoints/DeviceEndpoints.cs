using Application.DTOs.DeviceDtos;
using Application.DTOs.UserDtos;
using Application.Interfaces;
using Application.Validators;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Presentation.Endpoints;

public static class DeviceEndpoints
{
    public static void MapDeviceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/devices").WithTags("Devices");

        group.MapPost("", async (
            CreateDeviceRequestDto createDeviceRequest,
            IDeviceService deviceService,
            IValidator<CreateDeviceRequestDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(createDeviceRequest);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            try
            {
                await deviceService.CreateDeviceAsync(createDeviceRequest);
                return Results.Created($"/api/devices/", new { Message = "Device created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { ex.Message });
            }
        }).RequireAuthorization();

        group.MapGet("/", async (IDeviceService deviceService) =>
        {
            var devices = await deviceService.GetAllDevicesAsync();
            return Results.Ok(devices);
        }).RequireAuthorization();

        group.MapGet("/unassigned", async (IDeviceService deviceService) =>
        {
            var devices = await deviceService.GetUnassignedDevicesAsync();
            return Results.Ok(devices);
        }).RequireAuthorization();

        group.MapGet("/my-devices", async (IDeviceService deviceService, ClaimsPrincipal user) =>
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Results.Unauthorized();
            }

            var devices = await deviceService.GetDevicesByUserIdAsync(userId);
            return Results.Ok(devices);
        }).RequireAuthorization();

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
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { ex.Message });
            }
        }).RequireAuthorization();

        group.MapPatch("/{id:int}", async (
            int id,
            UpdateDeviceRequestDto updateDeviceDto,
            IDeviceService deviceService,
            IValidator<UpdateDeviceRequestDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(updateDeviceDto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                await deviceService.UpdateDeviceAsync(id, updateDeviceDto);
                return Results.Ok(new { Message = "Device updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { ex.Message });
            }
        }).RequireAuthorization();

        group.MapDelete("/{id:int}", async (int id, IDeviceService deviceService) =>
        {
            try
            {
                await deviceService.DeleteDeviceAsync(id);
                return Results.Ok(new { Message = "Device deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { ex.Message });
            }
        }).RequireAuthorization();
    }
}
