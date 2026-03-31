using Application.DTOs.DeviceDtos;

namespace Application.Interfaces;

public interface IDeviceService
{
    Task<IEnumerable<DeviceResponseDto>> GetAllDevicesAsync();
    Task<DeviceResponseDto> GetDeviceByIdAsync(int id);
    Task CreateDeviceAsync(CreateDeviceRequestDto createDeviceDto);
    Task UpdateDeviceAsync(int id, UpdateDeviceRequestDto updateDeviceDto);
    Task DeleteDeviceAsync(int id);
}
