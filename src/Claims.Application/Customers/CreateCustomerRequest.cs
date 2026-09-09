using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Customers;

public record CreateCustomerRequest(
    [property: Required]
    [property: StringLength(100, MinimumLength = 1)]
    string FirstName,

    [property: Required]
    [property: StringLength(100, MinimumLength = 1)]
    string LastName,

    [property: Required]
    [property: EmailAddress]
    [property: StringLength(255)]
    string Email,

    [property: StringLength(30)]
    string PhoneNumber
);
