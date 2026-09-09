using Claims.Application.Policies;
using Claims.Domain.Entities;
using Claims.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class PolicyRepository : IPolicyRepository
{
    private readonly ClaimsDbContext _dbContext;

    public PolicyRepository(ClaimsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Policy?> GetByPolicyNumberAsync(
        string policyNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Policies
            .FirstOrDefaultAsync(
                policy => policy.PolicyNumber == policyNumber,
                cancellationToken);
    }

    public async Task AddAsync(
        Policy policy,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Policies.AddAsync(
            policy,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
