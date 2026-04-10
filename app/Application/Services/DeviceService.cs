using Domain.Models;
using Domain.Interfaces;
using Application.Interfaces;
using Application.DTOs.DeviceDtos;

namespace Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDeviceDescriptionGenerator _deviceDescriptionGenerator;
    public DeviceService(
        IDeviceRepository deviceRepository, 
        IUserRepository userRepository,
        IDeviceDescriptionGenerator deviceDescriptionGenerator)
    {
        _deviceRepository = deviceRepository;
        _userRepository = userRepository;
        _deviceDescriptionGenerator = deviceDescriptionGenerator;
    }

    public async Task CreateDeviceAsync(CreateDeviceRequestDto createDeviceDto)
    {
        int? userId = createDeviceDto.UserId == 0 ? null : createDeviceDto.UserId;

        if (userId.HasValue)
        {
            var user = await _userRepository.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                throw new ArgumentException($"Assignment failed: User with ID {userId.Value} does not exist.");
            }
        }

        if (await _deviceRepository.DeviceExistsAsync(createDeviceDto.Name, createDeviceDto.Manufacturer, userId))
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
            UserId = userId
        });
    }

    public async Task DeleteDeviceAsync(int id, int currentUserId)
    {
        Device? existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
        if (existingDevice == null) {
            throw new KeyNotFoundException($"Device with ID {id} not found.");
        }
        if( existingDevice.UserId.HasValue && existingDevice.UserId != currentUserId) {
            throw new UnauthorizedAccessException("You do not have permission to delete this device.");
        }
        await _deviceRepository.DeleteDeviceAsync(id);
    }

    public async Task<IEnumerable<DeviceResponseDto>> GetAllDevicesAsync(int page = 1, int pageSize = 10)
    {
        IEnumerable<Device> devices = await _deviceRepository.GetAllDevicesAsync(page, pageSize);
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
            UserId = d.UserId ?? 0,
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
    public async Task UpdateDeviceAsync(int id, UpdateDeviceRequestDto updateDeviceDto, int currentUserId)
    {
        Device? existingDevice = await _deviceRepository.GetDeviceByIdAsync(id);
        if (existingDevice == null)
        {
            throw new KeyNotFoundException($"Device with ID {id} not found.");
        }

        if (existingDevice.UserId.HasValue && existingDevice.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this device.");
        }

        if(!existingDevice.UserId.HasValue && updateDeviceDto.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to assign this device to another user.");
        }

        int? newUserId = updateDeviceDto.UserId ?? existingDevice.UserId;
        if (updateDeviceDto.UserId == 0)
        {
            newUserId = null;
        }

        if (newUserId.HasValue)
        {
            var user = await _userRepository.GetUserByIdAsync(newUserId.Value);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {newUserId.Value} does not exist.");
            }
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
            UserId = newUserId,
        };
        await _deviceRepository.UpdateDeviceAsync(deviceToUpdate);

    }

    public async Task<IEnumerable<DeviceResponseDto>> GetDevicesByUserIdAsync(int userId, int page = 1, int pageSize = 10)
    {
        var devices = await _deviceRepository.GetDevicesByUserIdAsync(userId, page, pageSize);
        return MapToDeviceResponseDto(devices);
    }

    public async Task<IEnumerable<DeviceResponseDto>> GetUnassignedDevicesAsync(int page = 1, int pageSize = 10)
    {
        var devices = await _deviceRepository.GetUnassignedDevicesAsync(page, pageSize);
        return MapToDeviceResponseDto(devices);
    }

    public async Task<string> GetDeviceDescriptionAsync(int id)
    {
        Device? device = await _deviceRepository.GetDeviceByIdAsync(id);
        if(device == null)
        {
            throw new KeyNotFoundException($"Device with ID {id} not found.");
        }

        return await _deviceDescriptionGenerator.GenerateDescriptionAsync(device);
    }
    public async Task<IEnumerable<DeviceResponseDto>> SearchDeviceAsync(string query, int page = 1, int pageSize = 10)
    {
        var devices = await _deviceRepository.SearchDeviceAsync(query, page, pageSize);
        return MapToDeviceResponseDto(devices);
    }

    private IEnumerable<DeviceResponseDto> MapToDeviceResponseDto(IEnumerable<Device> devices)
    {
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
            UserId = d.UserId ?? 0,
            UserName = d.User?.Name ?? "Unassigned",
            UserRole = d.User?.Role ?? "N/A",
            UserLocation = d.User?.Location ?? "Storage"
        }).ToList();
    }

    
}