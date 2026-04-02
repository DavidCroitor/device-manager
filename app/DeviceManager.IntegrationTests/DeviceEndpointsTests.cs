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
        // Arrange - Create a test user first
        var userRequest = new CreateUserRequestDto
        {
            Name = "Test User",
            Role = "Developer",
            Location = "Office"
        };
        var userResponse = await _client.PostAsJsonAsync("/api/users", userRequest);
        userResponse.EnsureSuccessStatusCode();

        var request = new CreateDeviceRequestDto
        {
            Name = "Test iPhone",
            Manufacturer = "Apple",
            Type = "phone",
            OS = "iOS",
            OSVersion = "17",
            Processor = "A17",
            RamGB = 8,
            Description = "Integration Test Device",
            UserId = 1  // Use the created user
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
    public async Task CreateDevice_WithoutUserId_ReturnsBadRequest()
    {
        var request = new CreateDeviceRequestDto
        {
            Name = "Test iPhone Without User",
            Manufacturer = "Apple",
            Type = "phone",
            OS = "iOS",
            OSVersion = "17",
            Processor = "A17",
            RamGB = 8,
            Description = "Integration Test Device",
            // UserId omitted (defaults to 0)
        };

        var response = await _client.PostAsJsonAsync("/api/devices", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDevice_WithNonExistentUser_ReturnsBadRequest()
    {
        var request = new CreateDeviceRequestDto
        {
            Name = "Test iPhone Bad User",
            Manufacturer = "Apple",
            Type = "phone",
            OS = "iOS",
            OSVersion = "17",
            Processor = "A17",
            RamGB = 8,
            Description = "Integration Test Device",
            UserId = 99999  // Non-existent user
        };

        var response = await _client.PostAsJsonAsync("/api/devices", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task CreateDevice_WhenDeviceAlreadyExists_ReturnsConflict()
    {
        // Arrange - Create a test user first
        var userRequest = new CreateUserRequestDto
        {
            Name = "Test User",
            Role = "Developer",
            Location = "Office"
        };
        var userResponse = await _client.PostAsJsonAsync("/api/users", userRequest);
        userResponse.EnsureSuccessStatusCode();

        var request = new CreateDeviceRequestDto
        {
            Name = "Existing Device Test",
            Manufacturer = "Test Manufacturer",
            Type = "phone",
            OS = "iOS",
            OSVersion = "17",
            Processor = "A17",
            RamGB = 8,
            Description = "Integration Test Device",
            UserId = 1  // Use the created user
        };

        // Seed the device first
        var seedResponse = await _client.PostAsJsonAsync("/api/devices", request);
        seedResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act
        var conflictResponse = await _client.PostAsJsonAsync("/api/devices", request);

        // Assert
        conflictResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetDeviceById_WhenNotExists_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/devices/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDeviceById_WhenExists_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/devices/1"); // Assuming device with ID 1 is seeded

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var device = await response.Content.ReadFromJsonAsync<DeviceResponseDto>();
        device.Should().NotBeNull();
        device.Id.Should().Be(1);
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
    public async Task UpdateDevice_WithInvalidData_ReturnsValidationProblem()
    {
        // Arrange - assuming RamGB can't be negative based on typical validation
        var updateRequest = new UpdateDeviceRequestDto
        {
            RamGB = -5
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/devices/1", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDevice_WhenNotExists_ReturnsNotFound()
    {
        // Arrange
        var updateRequest = new UpdateDeviceRequestDto
        {
            Name = "Updated Name",
            RamGB = 12
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/devices/99999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteDevice_ReturnsOk()
    {
        int idToDelete = 3;

        var response = await _client.DeleteAsync($"/api/devices/{idToDelete}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteDevice_WhenNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/devices/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}