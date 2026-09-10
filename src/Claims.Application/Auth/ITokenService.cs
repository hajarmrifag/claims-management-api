using Claims.Domain.Entities;

namespace Claims.Application.Auth;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
