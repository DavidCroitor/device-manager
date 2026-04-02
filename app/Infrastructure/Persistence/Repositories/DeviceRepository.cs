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
        command.Parameters.AddWithValue("@Description", device.Description);
        command.Parameters.AddWithValue("@UserId", device.UserId);

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

    public async Task<bool> DeviceExistsAsync(string name, string manufacturer, int userId)
    {
        using SqlConnection connection = new(_connectionString);
        const string existsQuery = "SELECT 1 FROM Devices WHERE Name = @Name AND Manufacturer = @Manufacturer AND UserId = @UserId";
        
        using SqlCommand command = new(existsQuery, connection);
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Manufacturer", manufacturer);
        command.Parameters.AddWithValue("@UserId", userId);

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
        command.Parameters.AddWithValue("@Description", device.Description);
        command.Parameters.AddWithValue("@UserId", device.UserId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
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
            Description = (string)reader["Description"],
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
        };
            device.User = new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                Name = (string)reader["UserName"],
                Role = (string)reader["Role"],
                Location = (string)reader["Location"]
            };

        return device;
    }
}