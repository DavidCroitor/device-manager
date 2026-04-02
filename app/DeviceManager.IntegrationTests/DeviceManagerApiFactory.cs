
namespace IntegrationTests;

public class DeviceManagerApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var testConn = "Server=(localdb)\\mssqllocaldb;Database=DeviceManager_INTEGRATION_TESTS;Trusted_Connection=True;TrustServerCertificate=True;";

        builder.UseSetting("ConnectionStrings:DefaultConnection", testConn);
    }
}