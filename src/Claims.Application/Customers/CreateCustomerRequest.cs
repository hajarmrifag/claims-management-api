namespace Claims.Application.Customers;

public record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber
);
