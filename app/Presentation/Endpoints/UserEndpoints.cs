using Application.DTOs.UserDtos;
using Application.Interfaces;
using FluentValidation;

namespace Presentation.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapPost("/register", async (
            CreateUserRequestDto createUserDto,
            IUserService userService,
            IValidator<CreateUserRequestDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(createUserDto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            await userService.CreateUserAsync(createUserDto);
            return Results.Created($"/api/users/", new { message = "User created successfully" });
        }).AllowAnonymous();

        group.MapPost("/login", async(
            LoginRequestDto loginRequest,
            IUserService userService,
            IValidator<LoginRequestDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(loginRequest);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            try
            {
                string token = await userService.LoginAsync(loginRequest);
                return Results.Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }

        }).AllowAnonymous();

        group.MapGet("/", async (IUserService userService) =>
        {
            var users = await userService.GetAllUsersAsync();
            return Results.Ok(users);
        }).RequireAuthorization();

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
        }).RequireAuthorization();

        group.MapPatch("/{id:int}", async (
            int id, 
            UpdateUserRequestDto updateUserDto, 
            IUserService userService,
            IValidator<UpdateUserRequestDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                await userService.UpdateUserAsync(id, updateUserDto);
                return Results.Ok(new {message = "User updated successfully"});
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { ex.Message });
            }
        }).RequireAuthorization();

        group.MapDelete("/{id:int}", async (int id, IUserService userService) =>
        {
            try
            {
                await userService.DeleteUserAsync(id);
                return Results.Ok(new {message = "User deleted successfully"});
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { ex.Message });
            }
        }).RequireAuthorization();
    }
}
