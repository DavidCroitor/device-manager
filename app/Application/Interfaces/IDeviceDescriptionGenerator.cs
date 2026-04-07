using Domain.Models;

namespace Application.Interfaces;

public interface IDeviceDescriptionGenerator
{
    Task<string> GenerateDescriptionAsync(Device device);
}
