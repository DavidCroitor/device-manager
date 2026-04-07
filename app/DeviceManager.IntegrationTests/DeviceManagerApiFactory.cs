
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;

namespace IntegrationTests;

public class DeviceManagerApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var testConn = "Server=(localdb)\\mssqllocaldb;Database=DeviceManager_INTEGRATION_TESTS;Trusted_Connection=True;TrustServerCertificate=True;";

        builder.UseSetting("ConnectionStrings:DefaultConnection", testConn);

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(defaultScheme: "TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "TestScheme", options => { });
        });
    }
}