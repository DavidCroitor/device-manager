using Xunit;
using Microsoft.Data.SqlClient;

namespace IntegrationTests;

public class TestDatabaseFixture : IAsyncLifetime
{
    public string DbName = "DeviceManager_INTEGRATION_TESTS";
    public string MasterConn = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
    public string TestConn => $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True;TrustServerCertificate=True;";

    public async Task InitializeAsync()
    {
        using var conn = new SqlConnection(MasterConn);
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
            IF EXISTS (SELECT name FROM sys.databases WHERE name = '{DbName}')
            BEGIN
                ALTER DATABASE [{DbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{DbName}];
            END
            CREATE DATABASE [{DbName}];"; 
        await cmd.ExecuteNonQueryAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<TestDatabaseFixture>
{
}