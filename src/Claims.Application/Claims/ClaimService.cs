using Claims.Domain.Entities;

namespace Claims.Application.Claims;

public class ClaimService
{
    private readonly IClaimRepository _repository;

    public ClaimService(IClaimRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClaimResponse> CreateAsync(
        CreateClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingClaim = await _repository.GetByClaimNumberAsync(
            request.ClaimNumber,
            cancellationToken);

        if (existingClaim is not null)
        {
            throw new InvalidOperationException(
                $"Claim number '{request.ClaimNumber}' already exists.");
        }

        var claim = new Claim(
            request.ClaimNumber,
            request.CustomerId,
            request.PolicyId,
            request.Description,
            request.Amount);

        await _repository.AddAsync(claim, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return ToResponse(claim);
    }

    public async Task<ClaimResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var claim = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        return claim is null
            ? null
            : ToResponse(claim);
    }

    private static ClaimResponse ToResponse(Claim claim)
    {
        return new ClaimResponse(
            claim.Id,
            claim.ClaimNumber,
            claim.CustomerId,
            claim.PolicyId,
            claim.Description,
            claim.Amount,
            claim.Status.ToString(),
            claim.SubmittedAt);
    }
}
