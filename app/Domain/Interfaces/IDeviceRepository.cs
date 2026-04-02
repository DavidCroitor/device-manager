using Domain.Models;

namespace Domain.Interfaces;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllDevicesAsync();
    Task<Device?> GetDeviceByIdAsync(int id);
    Task AddDeviceAsync(Device device);
    Task UpdateDeviceAsync(Device device);
    Task DeleteDeviceAsync(int id);
    Task<bool> DeviceExistsAsync(string name, string manufacturer, int userId);
}