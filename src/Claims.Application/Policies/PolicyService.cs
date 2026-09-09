using Claims.Domain.Entities;

namespace Claims.Application.Policies;

public class PolicyService
{
    private readonly IPolicyRepository _repository;

    public PolicyService(IPolicyRepository repository)
    {
        _repository = repository;
    }

    public async Task<PolicyResponse> CreateAsync(
        CreatePolicyRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingPolicy = await _repository.GetByPolicyNumberAsync(
            request.PolicyNumber,
            cancellationToken);

        if (existingPolicy is not null)
        {
            throw new InvalidOperationException(
                $"Policy number '{request.PolicyNumber}' already exists.");
        }

        var policy = new Policy(
            request.PolicyNumber,
            request.CustomerId,
            request.PolicyType,
            request.CoverageAmount,
            request.StartDate,
            request.EndDate);

        await _repository.AddAsync(policy, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new PolicyResponse(
            policy.Id,
            policy.PolicyNumber,
            policy.CustomerId,
            policy.PolicyType,
            policy.CoverageAmount,
            policy.StartDate,
            policy.EndDate,
            policy.IsActive);
    }
}
