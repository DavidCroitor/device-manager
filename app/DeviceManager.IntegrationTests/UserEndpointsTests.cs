namespace IntegrationTests;

[Collection("Database collection")]
public class UserEndpointsTests : IClassFixture<DeviceManagerApiFactory>
{
    private readonly HttpClient _client;

    public UserEndpointsTests(DeviceManagerApiFactory factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task GetAllUsers_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();
        users.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUser_WithValidData_ReturnsCreated()
    {
        var request = new CreateUserRequestDto
        {
            Name = "Test User",
            Role = "Developer",
            Location = "Cluj"
        };

        var response = await _client.PostAsJsonAsync("/api/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ActionResponse>();
        result.Message.Should().Be("User created successfully");
    }

    [Fact]
    public async Task CreateUser_WithInvalidData_ReturnsValidationProblem()
    {
        var request = new CreateUserRequestDto { Name = "", Role = "" };

        var response = await _client.PostAsJsonAsync("/api/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUserById_WhenNotExists_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/users/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var result = await response.Content.ReadFromJsonAsync<ActionResponse>();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task UpdateUser_WithValidData_ReturnsOk()
    {
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "Updated User Name",
            Location = "Bucharest"
        };

        var response = await _client.PatchAsJsonAsync("/api/users/1", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ActionResponse>();
        result.Message.Should().Be("User updated successfully");
    }

    [Fact]
    public async Task DeleteUser_WhenExists_ReturnsOk()
    {
        var createRequest = new CreateUserRequestDto { Name = "Temp", Role = "Test", Location = "Test" };
        var createResponse = await _client.PostAsJsonAsync("/api/users", createRequest);

        int idToDelete = 2;

        var response = await _client.DeleteAsync($"/api/users/{idToDelete}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}