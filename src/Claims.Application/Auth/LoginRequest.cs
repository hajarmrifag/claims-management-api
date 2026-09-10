using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Auth;

public record LoginRequest(
    [param: Required]
    [param: EmailAddress]
    string Email,

    [param: Required]
    string Password
);
