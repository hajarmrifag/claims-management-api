using Claims.Domain.Entities;

namespace Claims.Application.Customers;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
