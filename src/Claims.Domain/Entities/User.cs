using Claims.Domain.Enums;

namespace Claims.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public UserRole Role { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private User()
    {
    }

    public User(
        string email,
        string passwordHash,
        UserRole role)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
}
