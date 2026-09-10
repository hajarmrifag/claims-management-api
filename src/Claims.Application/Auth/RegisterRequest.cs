using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Auth;

public record RegisterRequest(
    [param: Required]
    [param: EmailAddress]
    [param: MaxLength(255)]
    string Email,

    [param: Required]
    [param: MinLength(8)]
    [param: MaxLength(100)]
    string Password
);
