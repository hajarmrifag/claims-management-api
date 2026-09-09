using Claims.Application.Customers;
using Claims.Domain.Entities;
using Claims.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ClaimsDbContext _dbContext;

    public CustomerRepository(ClaimsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Customer?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .FirstOrDefaultAsync(
                customer => customer.Email == email,
                cancellationToken);
    }

    public async Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Customers.AddAsync(
            customer,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
