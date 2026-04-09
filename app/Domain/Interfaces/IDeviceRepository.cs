using Domain.Models;

namespace Domain.Interfaces;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllDevicesAsync(int page = 1, int pageSize = 10);
    Task<Device?> GetDeviceByIdAsync(int id);
    Task AddDeviceAsync(Device device);
    Task UpdateDeviceAsync(Device device);
    Task DeleteDeviceAsync(int id);
    Task<bool> DeviceExistsAsync(string name, string manufacturer, int? userId);
    Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId, int page = 1, int pageSize = 10);
    Task<IEnumerable<Device>> GetUnassignedDevicesAsync(int page = 1, int pageSize = 10);
    Task<IEnumerable<Device>> SearchDeviceAsync(string query, int page = 1, int pageSize = 10);
}
