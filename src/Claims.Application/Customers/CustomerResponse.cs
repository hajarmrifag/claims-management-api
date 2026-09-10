namespace Claims.Application.Customers;

public record CustomerResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    DateTime CreatedAt
);
