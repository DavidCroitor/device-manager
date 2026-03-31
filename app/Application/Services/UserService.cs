using Domain.Models;
using Domain.Interfaces;
using Application.Interfaces;
using Application.DTOs.UserDtos;

namespace Application.Services;

internal class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task CreateUserAsync(CreateUserRequestDto createUserDto)
    {
        await _userRepository.AddUserAsync(new User
        {
            Name = createUserDto.Name,
            Role = createUserDto.Role,
            Location = createUserDto.Location
        });
    }

    public async Task DeleteUserAsync(int id)
    {
        User? existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
            await _userRepository.DeleteUserAsync(id);
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        IEnumerable<User> users = await _userRepository.GetAllUsersAsync();
        return users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Name = u.Name,
            Role = u.Role,
            Location = u.Location
        });
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int id)
    {
        User? user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Role = user.Role,
            Location = user.Location
        };
    }

    public async Task UpdateUserAsync(int id, UpdateUserRequestDto updateUserDto)
    {
        User? existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"User with ID{id} not found");
        }
        if (updateUserDto.Name != null) existingUser.Name = updateUserDto.Name;
        if (updateUserDto.Role != null) existingUser.Role = updateUserDto.Role;
        if (updateUserDto.Location != null) existingUser.Location = updateUserDto.Location;

        await _userRepository.UpdateUserAsync(existingUser);

    }
}
