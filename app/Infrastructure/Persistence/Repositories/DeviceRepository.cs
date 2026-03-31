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
    public DeviceRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task AddDeviceAsync(Device device)
    {
        using SqlConnection connection = new(_connectionString);

        const string insertQuery =
            @"INSERT INTO Devices (Name, Manufacturer, Type, OS, OSVersion, Processor, RamGB, Description)
                VALUES (@Name, @Manufacturer, @Type, @OS, @OSVersion, @Processor, @RamGB, @Description);
                SELECT CAST(SCOPE_IDENTITY() as int);";

        using SqlCommand command = new SqlCommand(insertQuery, connection);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@Manufacturer", device.Manufacturer);
        command.Parameters.AddWithValue("@Type", device.Type);
        command.Parameters.AddWithValue("@OS", device.OS);
        command.Parameters.AddWithValue("@OSVersion", device.OSVersion);
        command.Parameters.AddWithValue("@Processor", device.Processor);
        command.Parameters.AddWithValue("@RamGB", device.RamGB);
        command.Parameters.AddWithValue("@Description", device.Description);

        connection.Open();
        await command.ExecuteNonQueryAsync();

    }

    public async Task DeleteDeviceAsync(int id)
    {
        using SqlConnection connection = new(_connectionString);
        const string deleteQuery = "DELETE FROM Devices WHERE Id = @Id";
        using SqlCommand command = new(deleteQuery, connection);
        command.Parameters.AddWithValue("@Id", id);
        connection.Open();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<Device>> GetAllDevicesAsync()
    {
        using SqlConnection connection = new(_connectionString);
        List<Device> devices = [];

        const string selectQuery = "SELECT Id, Name, Manufacturer, Type, OS, OSVersion, Processor, RamGB, Description FROM Devices";
        using SqlCommand command = new(selectQuery, connection);
        
        connection.Open();

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
        const string selectQuery = "SELECT Id, Name, Manufacturer, Type, OS, OSVersion, Processor, RamGB, Description FROM Devices WHERE Id = @Id";

        using SqlCommand command = new(selectQuery, connection);
        command.Parameters.AddWithValue("@Id", id);

        connection.Open();

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
                Description = @Description
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
        
        connection.Open();
        await command.ExecuteNonQueryAsync();
    }

    private Device MapToDevice(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        Name = (string)reader["Name"],
        Manufacturer = (string)reader["Manufacturer"],
        Type = (string)reader["Type"],
        OS = (string)reader["OS"],
        OSVersion = (string)reader["OSVersion"],
        Processor = (string)reader["Processor"],
        RamGB = reader.GetInt32(reader.GetOrdinal("RamGB")),
        Description = (string)reader["Description"]
    };
}