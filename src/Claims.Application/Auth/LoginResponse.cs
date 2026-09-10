namespace Claims.Application.Auth;

public record LoginResponse(
    string Token,
    DateTime ExpiresAt,
    string Email,
    string Role
);
