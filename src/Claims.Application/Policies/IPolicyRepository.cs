using Claims.Domain.Entities;

namespace Claims.Application.Policies;

public interface IPolicyRepository
{
    Task<Policy?> GetByPolicyNumberAsync(
        string policyNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Policy policy,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
