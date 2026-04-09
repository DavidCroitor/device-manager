using System.Data;
using System.Text;
using System.Text.RegularExpressions;
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

    public async Task<IEnumerable<Device>> GetAllDevicesAsync(int page = 1, int pageSize = 10)
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
            ORDER BY d.Id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        using SqlCommand command = new(selectQuery, connection);
        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        command.Parameters.AddWithValue("@PageSize", pageSize);

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

    public async Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId, int page = 1, int pageSize = 10)
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
            WHERE d.UserId = @UserId
            ORDER BY d.Id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        using SqlCommand command = new(selectQuery, connection);
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        command.Parameters.AddWithValue("@PageSize", pageSize);

        await connection.OpenAsync();

        using SqlDataReader reader = command.ExecuteReader();
        while (await reader.ReadAsync())
        {
            devices.Add(MapToDevice(reader));
        }

        return devices;
    }

    public async Task<IEnumerable<Device>> GetUnassignedDevicesAsync(int page = 1, int pageSize = 10)
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
            WHERE d.UserId IS NULL
            ORDER BY d.Id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        using SqlCommand command = new(selectQuery, connection);
        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        command.Parameters.AddWithValue("@PageSize", pageSize);

        await connection.OpenAsync();

        using SqlDataReader reader = command.ExecuteReader();
        while (await reader.ReadAsync())
        {
            devices.Add(MapToDevice(reader));
        }

        return devices;
    }

    public async Task<IEnumerable<Device>> SearchDeviceAsync(string query, int page = 1, int pageSize = 10)
    {
        if(string.IsNullOrWhiteSpace(query))
        {
            return await GetAllDevicesAsync();
        }
            
        List<Device> devices = [];
        string cleanQuery = Regex.Replace(query, @"[^\w\s]", "");
        string[] tokens = cleanQuery.Split(new[] { ' '}, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length == 0)
        {
            return await GetAllDevicesAsync();
        }

        using SqlConnection connection = new(_connectionString);
        var sql = new StringBuilder();
        sql.AppendLine(
            @"SELECT d.Id, d.Name, d.Manufacturer, d.Type, d.OS, d.OSVersion, d.Processor, d.RamGB, d.Description,
                     u.Id AS UserId,u.Name AS UserName, u.Role, u.Location, (");

        for (int i = 0; i < tokens.Length; i++)
        {
            string paramName = $"@t{i}";
            sql.AppendLine($"(CASE WHEN d.Name LIKE {paramName} THEN 10 ELSE 0 END) +");
            sql.AppendLine($"(CASE WHEN d.Manufacturer LIKE {paramName} THEN 5 ELSE 0 END) +");
            sql.AppendLine($"(CASE WHEN d.Processor LIKE {paramName} THEN 3 ELSE 0 END) +");
            sql.AppendLine($"(CASE WHEN d.RamGB LIKE {paramName} THEN 2 ELSE 0 END)" + (i == tokens.Length - 1 ? "" : " +"));
        }

        sql.AppendLine(") AS RelevanceScore");
        sql.AppendLine("FROM Devices d");
        sql.AppendLine("LEFT JOIN Users u ON d.UserId = u.Id");

        sql.AppendLine("WHERE");
        for (int i = 0; i < tokens.Length; i++)
        {
            string paramName = $"@t{i}";
            sql.AppendLine($"(d.Name LIKE {paramName} OR d.Manufacturer LIKE {paramName} OR d.Processor LIKE {paramName} OR d.RamGB LIKE {paramName})");
            if (i < tokens.Length - 1)
            {
                sql.AppendLine(" OR ");
            }
        }

        sql.AppendLine("ORDER BY RelevanceScore DESC, Name ASC");
        sql.AppendLine("OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");


        SqlCommand command = new(sql.ToString(), connection);
        for (int i = 0; i < tokens.Length; i++)
        {
            command.Parameters.AddWithValue($"@t{i}", $"%{tokens[i]}%");
        }

        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        command.Parameters.AddWithValue("@PageSize", pageSize);


        await connection.OpenAsync();
        using SqlDataReader reader = await command.ExecuteReaderAsync();

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