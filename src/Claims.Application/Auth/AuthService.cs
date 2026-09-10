using Claims.Domain.Entities;
using Claims.Domain.Enums;

namespace Claims.Application.Auth;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var existingUser = await _userRepository.GetByEmailAsync(
            email,
            cancellationToken);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var user = new User(
            email,
            string.Empty,
            UserRole.Adjuster);

        var passwordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

        user.SetPasswordHash(passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return CreateLoginResponse(user);
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(
            email,
            cancellationToken);

        if (user is null ||
            !_passwordHasher.VerifyPassword(
                user,
                user.PasswordHash,
                request.Password))
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        return CreateLoginResponse(user);
    }

    private LoginResponse CreateLoginResponse(User user)
    {
        var (token, expiresAt) = _tokenService.CreateToken(user);

        return new LoginResponse(
            token,
            expiresAt,
            user.Email,
            user.Role.ToString());
    }
}
