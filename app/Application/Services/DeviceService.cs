using Domain.Models;
using Domain.Interfaces;
using Application.Interfaces;
using Application.DTOs.DeviceDtos;

namespace Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IUserRepository _userRepository;
    public DeviceService(IDeviceRepository deviceRepository, IUserRepository userRepository)
    {
        _deviceRepository = deviceRepository;
        _userRepository = userRepository;
    }

    public async Task CreateDeviceAsync(CreateDeviceRequestDto createDeviceDto)
    {
        if (createDeviceDto.UserId == 0)
        {
            throw new ArgumentException("UserId is required.");
        }

        var user = await _userRepository.GetUserByIdAsync(createDeviceDto.UserId);
        if (user == null)
        {
            throw new ArgumentException($"Assignment failed: User with ID {createDeviceDto.UserId} does not exist.");
        }

        if (await _deviceRepository.DeviceExistsAsync(createDeviceDto.Name, createDeviceDto.Manufacturer, createDeviceDto.UserId))
        {
            throw new InvalidOperationException("A device with the same user, name and manufacturer already exists.");
        }

        await _deviceRepository.AddDeviceAsync(new Device
        {
            Name = createDeviceDto.Name,
            Manufacturer = createDeviceDto.Manufacturer,
            Type = createDeviceDto.Type,
            OS = createDeviceDto.OS,
            OSVersion = createDeviceDto.OSVersion,
            Processor = createDeviceDto.Processor,
            RamGB = createDeviceDto.RamGB,
            Description = createDeviceDto.Description,
            UserId = createDeviceDto.UserId
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
            Description = d.Description,
            UserName =  d.User?.Name ?? "Unassigned",
            UserRole = d.User?.Role ?? "N/A",
            UserLocation = d.User?.Location ?? "Storage"
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
            Description = device.Description,
            UserName = device.User?.Name ?? "Unassigned",
            UserRole = device.User?.Role ?? "N/A",
            UserLocation = device.User?.Location ?? "Storage"
        };
    }
    public async Task UpdateDeviceAsync(int id, UpdateDeviceRequestDto updateDeviceDto)
    {
        Device? existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
        if (existingDevice == null)
        {
            throw new KeyNotFoundException($"Device with ID {id} not found.");
        }

        var user = await _userRepository.GetUserByIdAsync(existingDevice.UserId);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {existingDevice.UserId} does not exist.");
        }

        var deviceToUpdate = new Device
        {
            Id = id,
            Name = updateDeviceDto.Name ?? existingDevice.Name,
            Manufacturer = updateDeviceDto.Manufacturer ?? existingDevice.Manufacturer,
            Type = updateDeviceDto.Type ?? existingDevice.Type,
            OS = updateDeviceDto.OS ?? existingDevice.OS,
            OSVersion = updateDeviceDto.OSVersion ?? existingDevice.OSVersion,
            Processor = updateDeviceDto.Processor ?? existingDevice.Processor,
            RamGB = updateDeviceDto.RamGB != 0 ? updateDeviceDto.RamGB : existingDevice.RamGB,
            Description = updateDeviceDto.Description ?? existingDevice.Description,
            UserId = existingDevice.UserId,
        };
        await _deviceRepository.UpdateDeviceAsync(deviceToUpdate);

    }
}