using Application.DTOs.UserDtos;

namespace Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto> GetUserByIdAsync(int id);
    Task CreateUserAsync(CreateUserRequestDto createUserDto);
    Task UpdateUserAsync(int id, UpdateUserRequestDto updateUserDto);
    Task DeleteUserAsync(int id);
}
