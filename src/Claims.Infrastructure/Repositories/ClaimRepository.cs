using Claims.Application.Claims;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Claims.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class ClaimRepository : IClaimRepository
{
    private readonly ClaimsDbContext _dbContext;

    public ClaimRepository(ClaimsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Claim?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Claims
            .FirstOrDefaultAsync(
                claim => claim.Id == id,
                cancellationToken);
    }

    public async Task<Claim?> GetByClaimNumberAsync(
        string claimNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Claims
            .FirstOrDefaultAsync(
                claim => claim.ClaimNumber == claimNumber,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<Claim> Items, int TotalCount)> SearchAsync(
        ClaimQuery query,
        CancellationToken cancellationToken = default)
    {
        var claims = _dbContext.Claims
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            claims = claims.Where(claim =>
                EF.Functions.Like(
                    claim.ClaimNumber,
                    $"%{search}%") ||
                EF.Functions.Like(
                    claim.Description,
                    $"%{search}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<ClaimStatus>(
                query.Status,
                true,
                out var status))
        {
            claims = claims.Where(claim => claim.Status == status);
        }

        if (query.FromDate.HasValue)
        {
            var fromDate = query.FromDate.Value.Date;

            claims = claims.Where(
                claim => claim.SubmittedAt >= fromDate);
        }

        if (query.ToDate.HasValue)
        {
            var exclusiveEndDate = query.ToDate.Value.Date.AddDays(1);

            claims = claims.Where(
                claim => claim.SubmittedAt < exclusiveEndDate);
        }

        var totalCount = await claims.CountAsync(cancellationToken);

        var items = await claims
            .OrderByDescending(claim => claim.SubmittedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<ClaimDocument?> GetDocumentByIdAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClaimDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                document => document.Id == documentId,
                cancellationToken);
    }

    public async Task AddAsync(
        Claim claim,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Claims.AddAsync(
            claim,
            cancellationToken);
    }

    public async Task AddStatusHistoryAsync(
        ClaimStatusHistory history,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.ClaimStatusHistories.AddAsync(
            history,
            cancellationToken);
    }

    public async Task AddDocumentAsync(
        ClaimDocument document,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.ClaimDocuments.AddAsync(
            document,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
