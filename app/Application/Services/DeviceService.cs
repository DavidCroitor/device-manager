using Domain.Models;
using Domain.Interfaces;
using Application.Interfaces;
using Application.DTOs.DeviceDtos;

namespace Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task CreateDeviceAsync(CreateDeviceRequestDto createDeviceDto)
    {
        await _deviceRepository.AddDeviceAsync(new Device
        {
            Name = createDeviceDto.Name,
            Manufacturer = createDeviceDto.Manufacturer,
            Type = createDeviceDto.Type,
            OS = createDeviceDto.OS,
            OSVersion = createDeviceDto.OSVersion,
            Processor = createDeviceDto.Processor,
            RamGB = createDeviceDto.RamGB,
            Description = createDeviceDto.Description
        });
    }

    public async Task DeleteDeviceAsync(int id)
    {
        Device? existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
        if (existingDevice == null) {
            throw new KeyNotFoundException($"Device with ID {id} not found.");
        }
        await _deviceRepository.DeleteDeviceAsync(id);
    }

    public async Task<IEnumerable<DeviceResponseDto>> GetAllDevicesAsync()
    {
        IEnumerable<Device> devices = await _deviceRepository.GetAllDevicesAsync();
        return devices.Select(d => new DeviceResponseDto
        {
            Id = d.Id,
            Name = d.Name,
            Manufacturer = d.Manufacturer,
            Type = d.Type,
            OS = d.OS,
            OSVersion = d.OSVersion,
            Processor = d.Processor,
            RamGB = d.RamGB,
            Description = d.Description
        }).ToList();
    }

    public async Task<DeviceResponseDto> GetDeviceByIdAsync(int id)
    {
        Device? device = await _deviceRepository.GetDeviceByIdAsync(id);
        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID {id} not found.");
        }
        return new DeviceResponseDto
        {
            Id = device.Id,
            Name = device.Name,
            Manufacturer = device.Manufacturer,
            Type = device.Type,
            OS = device.OS,
            OSVersion = device.OSVersion,
            Processor = device.Processor,
            RamGB = device.RamGB,
            Description = device.Description
        };
    }

    public async Task UpdateDeviceAsync(int id, UpdateDeviceRequestDto updateDeviceDto)
    {
        Device? existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
        if (existingDevice == null)
        {
            throw new KeyNotFoundException($"Device with ID {id} not found.");
        }
        if (updateDeviceDto.Name != null) existingDevice.Name = updateDeviceDto.Name;
        if (updateDeviceDto.Manufacturer != null) existingDevice.Manufacturer = updateDeviceDto.Manufacturer;
        if (updateDeviceDto.Type != null) existingDevice.Type = updateDeviceDto.Type;
        if (updateDeviceDto.OS != null) existingDevice.OS = updateDeviceDto.OS;
        if (updateDeviceDto.OSVersion != null) existingDevice.OSVersion = updateDeviceDto.OSVersion;
        if (updateDeviceDto.Processor != null) existingDevice.Processor = updateDeviceDto.Processor;
        if (updateDeviceDto.RamGB != 0) existingDevice.RamGB = updateDeviceDto.RamGB;
        if (updateDeviceDto.Description != null) existingDevice.Description = updateDeviceDto.Description;
        await _deviceRepository.UpdateDeviceAsync(existingDevice);

    }
}