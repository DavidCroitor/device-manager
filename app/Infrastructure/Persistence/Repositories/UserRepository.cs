using Domain.Interfaces;
using Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
    public async Task AddUserAsync(User user)
    {
        using SqlConnection connection = new(_connectionString);
        const string insertQuery = 
            @"INSERT INTO Users (Name, Role, Location) 
                VALUES (@Name, @Role, @Location); 
                SELECT CAST(SCOPE_IDENTITY() as int);";

        using SqlCommand insertCommand = new(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@Name", user.Name);
        insertCommand.Parameters.AddWithValue("@Role", user.Role);
        insertCommand.Parameters.AddWithValue("@Location", user.Location);

        connection.Open();
        await insertCommand.ExecuteNonQueryAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        using SqlConnection connection = new(_connectionString);
        const string deleteQuery = "DELETE FROM Users WHERE Id = @Id";

        using SqlCommand command = new(deleteQuery, connection);
        command.Parameters.AddWithValue("@Id", id);

        connection.Open();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        using SqlConnection connection = new(_connectionString);
        List<User> users = [];

        const string selectQuery = "SELECT Id, Name, Role, Location FROM Users";
        using SqlCommand command = new(selectQuery, connection);

        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        while (await reader.ReadAsync())
        {
            users.Add(MapToUser(reader));
        }
        return users;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        using SqlConnection connection = new(_connectionString);
        const string selectQuery = "SELECT Id, Name, Role, Location FROM Users WHERE Id = @Id";
        
        using SqlCommand command = new(selectQuery, connection);
        command.Parameters.AddWithValue("@Id", id);

        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        if(await reader.ReadAsync())
        {
            return MapToUser(reader);
        }

        return null;
    }

    public async Task UpdateUserAsync(User user)
    {
        using SqlConnection connection = new(_connectionString);
        const string updateQuery = 
            @"UPDATE Users 
            SET Name = @Name, 
                Role = @Role, 
                Location = @Location 
            WHERE Id = @Id";

        using SqlCommand command = new(updateQuery, connection);
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Role", user.Role);
        command.Parameters.AddWithValue("@Location", user.Location);

        connection.Open();
        await command.ExecuteNonQueryAsync();

    }

    private static User MapToUser(SqlDataReader reader) => new()
    {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = (string)reader["Name"],
            Role = (string)reader["Role"],
            Location = (string)reader["Location"]
    };
}
