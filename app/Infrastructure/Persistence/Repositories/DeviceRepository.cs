using System.Data;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure.Persistence.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly string _connectionString;
    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AddDeviceAsync(Device device)
    {
        using SqlConnection connection = new(_connectionString);

        const string insertQuery =
            @"INSERT INTO Devices (Name, Manufacturer, Type, OS, OSVersion, Processor, RamGB, Description, UserId)
                VALUES (@Name, @Manufacturer, @Type, @OS, @OSVersion, @Processor, @RamGB, @Description, @UserId)";

        using SqlCommand command = new SqlCommand(insertQuery, connection);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@Manufacturer", device.Manufacturer);
        command.Parameters.AddWithValue("@Type", device.Type);
        command.Parameters.AddWithValue("@OS", device.OS);
        command.Parameters.AddWithValue("@OSVersion", device.OSVersion);
        command.Parameters.AddWithValue("@Processor", device.Processor);
        command.Parameters.AddWithValue("@RamGB", device.RamGB);
        command.Parameters.AddWithValue("@Description", device.Description ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@UserId", device.UserId ?? (object)DBNull.Value);

        connection.Open();
        await command.ExecuteNonQueryAsync();

    }

    public async Task DeleteDeviceAsync(int id)
    {
        using SqlConnection connection = new(_connectionString);
        const string deleteQuery = "DELETE FROM Devices WHERE Id = @Id";
        using SqlCommand command = new(deleteQuery, connection);
        command.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> DeviceExistsAsync(string name, string manufacturer, int? userId)
    {
        using SqlConnection connection = new(_connectionString);
        string existsQuery = "SELECT 1 FROM Devices WHERE Name = @Name AND Manufacturer = @Manufacturer " + 
                             (userId.HasValue ? "AND UserId = @UserId" : "AND UserId IS NULL");

        using SqlCommand command = new(existsQuery, connection);
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Manufacturer", manufacturer);
        if (userId.HasValue)
        {
            command.Parameters.AddWithValue("@UserId", userId.Value);
        }

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();
        return result != null;

    }

    public async Task<IEnumerable<Device>> GetAllDevicesAsync()
    {
        using SqlConnection connection = new(_connectionString);
        List<Device> devices = [];

        const string selectQuery =
            @"SELECT 
                d.Id, d.Name, d.Manufacturer, d.Type, d.OS, d.OSVersion, d.Processor, d.RamGB, d.Description,
                u.Id AS UserId, 
                u.Name AS UserName, 
                u.Role, 
                u.Location
            FROM Devices d
            LEFT JOIN Users u ON d.UserId = u.Id";
        using SqlCommand command = new(selectQuery, connection);
        
        await connection.OpenAsync();

        using SqlDataReader reader = command.ExecuteReader();
        while (await reader.ReadAsync())
        {
            devices.Add(MapToDevice(reader));
        }

        return devices;
    }

    public async Task<Device?> GetDeviceByIdAsync(int id)
    {
        using SqlConnection connection = new(_connectionString);
        const string selectQuery =
            @"SELECT 
                d.Id, d.Name, d.Manufacturer, d.Type, d.OS, d.OSVersion, d.Processor, d.RamGB, d.Description,
                u.Id AS UserId, 
                u.Name AS UserName, 
                u.Role, 
                u.Location
            FROM Devices d
            LEFT JOIN Users u ON d.UserId = u.Id
            WHERE d.Id = @Id";

        using SqlCommand command = new(selectQuery, connection);
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();

        using SqlDataReader reader = command.ExecuteReader();
        if(await reader.ReadAsync())
        {
            return MapToDevice(reader);
        }
        return null;
    }

    public async Task UpdateDeviceAsync(Device device)
    {
        using SqlConnection connection = new(_connectionString);
        const string updateQuery =
            @"UPDATE Devices 
            SET Name = @Name, 
                Manufacturer = @Manufacturer, 
                Type = @Type, 
                OS = @OS, 
                OSVersion = @OSVersion, 
                Processor = @Processor, 
                RamGB = @RamGB, 
                Description = @Description,
                UserId = @UserId
            WHERE Id = @Id";
    
        using SqlCommand command = new(updateQuery, connection);
        command.Parameters.AddWithValue("@Id", device.Id);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@Manufacturer", device.Manufacturer);
        command.Parameters.AddWithValue("@Type", device.Type);
        command.Parameters.AddWithValue("@OS", device.OS);
        command.Parameters.AddWithValue("@OSVersion", device.OSVersion);
        command.Parameters.AddWithValue("@Processor", device.Processor);
        command.Parameters.AddWithValue("@RamGB", device.RamGB);
        command.Parameters.AddWithValue("@Description", device.Description ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@UserId", device.UserId ?? (object)DBNull.Value);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId)
    {
        using SqlConnection connection = new(_connectionString);
        List<Device> devices = [];

        const string selectQuery =
            @"SELECT 
                d.Id, d.Name, d.Manufacturer, d.Type, d.OS, d.OSVersion, d.Processor, d.RamGB, d.Description,
                u.Id AS UserId, 
                u.Name AS UserName, 
                u.Role, 
                u.Location
            FROM Devices d
            LEFT JOIN Users u ON d.UserId = u.Id
            WHERE d.UserId = @UserId";
        using SqlCommand command = new(selectQuery, connection);
        command.Parameters.AddWithValue("@UserId", userId);
        
        await connection.OpenAsync();

        using SqlDataReader reader = command.ExecuteReader();
        while (await reader.ReadAsync())
        {
            devices.Add(MapToDevice(reader));
        }

        return devices;
    }

    public async Task<IEnumerable<Device>> GetUnassignedDevicesAsync()
    {
        using SqlConnection connection = new(_connectionString);
        List<Device> devices = [];

        const string selectQuery =
            @"SELECT 
                d.Id, d.Name, d.Manufacturer, d.Type, d.OS, d.OSVersion, d.Processor, d.RamGB, d.Description,
                u.Id AS UserId, 
                u.Name AS UserName, 
                u.Role, 
                u.Location
            FROM Devices d
            LEFT JOIN Users u ON d.UserId = u.Id
            WHERE d.UserId IS NULL";
        using SqlCommand command = new(selectQuery, connection);
        
        await connection.OpenAsync();

        using SqlDataReader reader = command.ExecuteReader();
        while (await reader.ReadAsync())
        {
            devices.Add(MapToDevice(reader));
        }

        return devices;
    }

    private Device MapToDevice(SqlDataReader reader) 
    {
        var device = new Device
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = (string)reader["Name"],
            Manufacturer = (string)reader["Manufacturer"],
            Type = (string)reader["Type"],
            OS = (string)reader["OS"],
            OSVersion = (string)reader["OSVersion"],
            Processor = (string)reader["Processor"],
            RamGB = reader.GetInt32(reader.GetOrdinal("RamGB")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : (string)reader["Description"],
            UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? null : reader.GetInt32(reader.GetOrdinal("UserId")),
        };

        if (device.UserId.HasValue)
        {
            device.User = new User
            {
                Id = device.UserId.Value,
                Name = reader.IsDBNull(reader.GetOrdinal("UserName")) ? string.Empty : (string)reader["UserName"],
                Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? string.Empty : (string)reader["Role"],
                Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? string.Empty : (string)reader["Location"]
            };
        }

        return device;
    }
}