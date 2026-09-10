using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Customers;

public record CreateCustomerRequest(
    [param: Required]
    [param: StringLength(100, MinimumLength = 1)]
    string FirstName,

    [param: Required]
    [param: StringLength(100, MinimumLength = 1)]
    string LastName,

    [param: Required]
    [param: EmailAddress]
    [param: StringLength(255)]
    string Email,

    [param: StringLength(30)]
    string? PhoneNumber
);
