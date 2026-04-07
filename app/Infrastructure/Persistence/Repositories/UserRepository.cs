using Domain.Interfaces;
using Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task AddUserAsync(User user)
    {
        using SqlConnection connection = new(_connectionString);
        const string insertQuery = 
            @"INSERT INTO Users (Email, Name, Role, Location, PasswordHash) 
                VALUES (@Email, @Name, @Role, @Location, @PasswordHash); 
                SELECT CAST(SCOPE_IDENTITY() as int);";
        
        using SqlCommand insertCommand = new(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@Name", user.Name);
        insertCommand.Parameters.AddWithValue("@Email", user.Email);
        insertCommand.Parameters.AddWithValue("@Role", user.Role);
        insertCommand.Parameters.AddWithValue("@Location", user.Location);
        insertCommand.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

        await connection.OpenAsync();
        await insertCommand.ExecuteNonQueryAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        using SqlConnection connection = new(_connectionString);
        const string deleteQuery = "DELETE FROM Users WHERE Id = @Id";

        using SqlCommand command = new(deleteQuery, connection);
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        using SqlConnection connection = new(_connectionString);
        List<User> users = [];

        const string selectQuery = "SELECT Id, Name, Email, Role, Location, PasswordHash FROM Users";
        using SqlCommand command = new(selectQuery, connection);

        await connection.OpenAsync();
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
        const string selectQuery = "SELECT Id, Name, Email, Role, Location, PasswordHash FROM Users WHERE Id = @Id";
        
        using SqlCommand command = new(selectQuery, connection);
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
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
                Email = @Email,
                Role = @Role, 
                Location = @Location,
                PasswordHash = @PasswordHash
            WHERE Id = @Id";

        using SqlCommand command = new(updateQuery, connection);
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Role", user.Role);
        command.Parameters.AddWithValue("@Location", user.Location);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

    }
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        using SqlConnection connection = new(_connectionString);
        const string selectQuery = "SELECT Id, Name, Email, Role, Location, PasswordHash FROM Users WHERE Email = @Email";

        using SqlCommand command = new(selectQuery, connection);
        command.Parameters.AddWithValue("@Email", email);

        await connection.OpenAsync();
        using SqlDataReader reader = command.ExecuteReader();
        if (await reader.ReadAsync())
        {
            return MapToUser(reader);
        }

        return null;
    }

    private static User MapToUser(SqlDataReader reader) => new()
    {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Email = (string)reader["Email"],
            Name = (string)reader["Name"],
            Role = (string)reader["Role"],
            Location = (string)reader["Location"],
            PasswordHash = (string)reader["PasswordHash"]
    };

    
}
