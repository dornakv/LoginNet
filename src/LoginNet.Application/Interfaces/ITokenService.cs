using LoginNet.Domain.Entities;

namespace LoginNet.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
