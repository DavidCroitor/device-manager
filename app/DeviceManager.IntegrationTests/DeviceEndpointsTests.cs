namespace IntegrationTests;

[Collection("Database collection")]
public class DeviceEndpointsTests : IClassFixture<DeviceManagerApiFactory>
{
    private readonly HttpClient _client;

    public DeviceEndpointsTests(DeviceManagerApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllDevices_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/devices");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var devices = await response.Content.ReadFromJsonAsync<List<DeviceResponseDto>>();
        devices.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateDevice_WithValidData_ReturnsCreated()
    {
        var request = new CreateDeviceRequestDto
        {
            Name = "Test iPhone",
            Manufacturer = "Apple",
            Type = "phone",
            OS = "iOS",
            OSVersion = "17",
            Processor = "A17",
            RamGB = 8,
            Description = "Integration Test Device"
        };

        var response = await _client.PostAsJsonAsync("/api/devices", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ActionResponse>();
        result.Message.Should().Be("Device created successfully");
    }

    [Fact]
    public async Task CreateDevice_WithInvalidData_ReturnsValidationProblem()
    {
        var request = new CreateDeviceRequestDto { Name = "" };

        var response = await _client.PostAsJsonAsync("/api/devices", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDeviceById_WhenNotExists_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/devices/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDevice_WithValidData_ReturnsOk()
    {
        var updateRequest = new UpdateDeviceRequestDto
        {
            Name = "Updated Name",
            RamGB = 12
        };

        var response = await _client.PatchAsJsonAsync("/api/devices/1", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteDevice_ReturnsOk()
    {
        int idToDelete = 3;

        var response = await _client.DeleteAsync($"/api/devices/{idToDelete}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}