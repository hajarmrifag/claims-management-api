using Claims.Domain.Entities;

namespace Claims.Application.Customers;

public class CustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingCustomer = await _repository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (existingCustomer is not null)
        {
            throw new InvalidOperationException(
                $"A customer with email '{request.Email}' already exists.");
        }

        var customer = new Customer(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber);

        await _repository.AddAsync(customer, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.PhoneNumber,
            customer.CreatedAt);
    }
}
