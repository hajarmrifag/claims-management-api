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

    public async Task<PagedResult<ClaimResponse>> SearchAsync(
        ClaimQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var normalizedQuery = query with
        {
            Page = page,
            PageSize = pageSize
        };

        var (items, totalCount) = await _repository.SearchAsync(
            normalizedQuery,
            cancellationToken);

        var responses = items
            .Select(ToResponse)
            .ToList();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)pageSize);

        return new PagedResult<ClaimResponse>(
            responses,
            page,
            pageSize,
            totalCount,
            totalPages);
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
