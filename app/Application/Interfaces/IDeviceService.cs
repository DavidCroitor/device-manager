using Application.DTOs.DeviceDtos;

namespace Application.Interfaces;

public interface IDeviceService
{
    Task<IEnumerable<DeviceResponseDto>> GetAllDevicesAsync(int page = 1, int pageSize = 10);
    Task<DeviceResponseDto> GetDeviceByIdAsync(int id);
    Task CreateDeviceAsync(CreateDeviceRequestDto createDeviceDto);
    Task UpdateDeviceAsync(int id, UpdateDeviceRequestDto updateDeviceDto, int currentUserId);
    Task DeleteDeviceAsync(int id, int currentUserId);
    Task<IEnumerable<DeviceResponseDto>> GetDevicesByUserIdAsync(int userId, int page = 1, int pageSize = 10);
    Task<IEnumerable<DeviceResponseDto>> GetUnassignedDevicesAsync(int page = 1, int pageSize = 10);
    Task<IEnumerable<DeviceResponseDto>> SearchDeviceAsync(string query, int page = 1, int pageSize = 10);
    Task<string> GetDeviceDescriptionAsync(int id);
}
