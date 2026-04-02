using Domain.Models;

namespace Application.Interfaces;

public interface ITokenGenerator
{
    string GenerateToken(User user);

}
