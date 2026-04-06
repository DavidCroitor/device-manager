using Application.DTOs.UserDtos;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;


namespace Application.Services;

internal class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    public UserService(IUserRepository userRepository, ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
    }
    public async Task CreateUserAsync(CreateUserRequestDto createUserDto)
    {
        if (await _userRepository.GetUserByEmailAsync(createUserDto.Email) != null)
        {
            throw new InvalidOperationException("This email is already in use.");
        }
        string passwordHash = _passwordHasher.HashPassword(createUserDto.Password);
        await _userRepository.AddUserAsync(new User
        {
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            Role = createUserDto.Role,
            Location = createUserDto.Location,
            PasswordHash = passwordHash
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
        var userToUpdate = new User
        {
            Id = existingUser.Id,
            Name = updateUserDto.Name ?? existingUser.Name,
            Role = updateUserDto.Role ?? existingUser.Role,
            Location = updateUserDto.Location ?? existingUser.Location
        };

        await _userRepository.UpdateUserAsync(userToUpdate);
    }

    public async Task<UserResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);
        if(user == null || !_passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        string token = _tokenGenerator.GenerateToken(user);

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Location = user.Location,
            Token = token
        };
    }
}
