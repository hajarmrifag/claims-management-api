using Claims.Domain.Entities;

namespace Claims.Application.Claims;

public interface IClaimRepository
{
    Task<Claim?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Claim?> GetByClaimNumberAsync(
        string claimNumber,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Claim> Items, int TotalCount)> SearchAsync(
        ClaimQuery query,
        CancellationToken cancellationToken = default);

    Task<ClaimDocument?> GetDocumentByIdAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Claim claim,
        CancellationToken cancellationToken = default);

    Task AddStatusHistoryAsync(
        ClaimStatusHistory history,
        CancellationToken cancellationToken = default);

    Task AddDocumentAsync(
        ClaimDocument document,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
