using Claims.Domain.Entities;

namespace Claims.Application.Auth;

public interface IPasswordHasher
{
    string HashPassword(User user, string password);

    bool VerifyPassword(
        User user,
        string hashedPassword,
        string providedPassword);
}
