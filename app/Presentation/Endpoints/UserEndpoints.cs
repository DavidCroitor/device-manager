using Application.DTOs.UserDtos;
using Application.Interfaces;

namespace Presentation.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", async (IUserService userService) =>
        {
            var users = await userService.GetAllUsersAsync();
            return Results.Ok(users);
        });

        group.MapGet("/{id:int}", async (int id, IUserService userService) =>
        {
            try
            {
                var user = await userService.GetUserByIdAsync(id);
                return Results.Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { ex.Message });
            }
        });

        group.MapPost("/", async (CreateUserRequestDto createUserDto, IUserService userService) =>
        {
            await userService.CreateUserAsync(createUserDto);
            return Results.Created($"/api/users/", new {Message = "User created successfully"});
        });

        group.MapPatch("/{id:int}", async (int id, UpdateUserRequestDto updateUserDto, IUserService userService) =>
        {
            try
            {
                await userService.UpdateUserAsync(id, updateUserDto);
                return Results.Ok(new {Message = "User updated successfully"});
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { ex.Message });
            }
        });

        group.MapDelete("/{id:int}", async (int id, IUserService userService) =>
        {
            try
            {
                await userService.DeleteUserAsync(id);
                return Results.Ok(new {Message = "User deleted successfully"});
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { ex.Message });
            }
        });
    }
}
