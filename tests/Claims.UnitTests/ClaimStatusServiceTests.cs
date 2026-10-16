using Claims.Application.Claims;
using Claims.Domain.Entities;
using Claims.Domain.Enums;

namespace Claims.UnitTests;

public class ClaimStatusServiceTests
{
    [Theory]
    [InlineData("999")]
    [InlineData("2")]
    [InlineData("Submitted, Approved")]
    [InlineData("-1")]
    [InlineData("Approved, Paid")]
    [InlineData("not-a-status")]
    public async Task InvalidStatus_IsRejectedWithoutMutationOrHistory(string value)
    {
        var repo = new MemoryRepository();
        var service = new ClaimService(repo);
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UpdateStatusAsync(repo.Claim.Id, new UpdateClaimStatusRequest(value)));
        Assert.Equal(ClaimStatus.Submitted, repo.Claim.Status);
        Assert.Null(repo.Claim.UpdatedAt);
        Assert.Equal(0, repo.HistoryCount);
        Assert.Equal(0, repo.Saves);
    }

    [Fact]
    public async Task DefinedStatus_WritesOneHistory_AndRepeatedStatusIsNoOp()
    {
        var repo = new MemoryRepository();
        var service = new ClaimService(repo);
        await service.UpdateStatusAsync(repo.Claim.Id, new UpdateClaimStatusRequest("approved"));
        await service.UpdateStatusAsync(repo.Claim.Id, new UpdateClaimStatusRequest("Approved"));
        Assert.Equal(ClaimStatus.Approved, repo.Claim.Status);
        Assert.Equal(1, repo.HistoryCount);
        Assert.Equal(1, repo.Saves);
    }

    private sealed class MemoryRepository : IClaimRepository
    {
        public Claim Claim { get; } = new("CLM-service", Guid.NewGuid(), Guid.NewGuid(), "Test", 1m);
        public int HistoryCount { get; private set; }
        public int Saves { get; private set; }
        public Task<Claim?> GetByIdAsync(Guid id, CancellationToken ct = default) => Task.FromResult<Claim?>(Claim);
        public Task<Claim?> GetByClaimNumberAsync(string number, CancellationToken ct = default) => Task.FromResult<Claim?>(null);
        public Task<(IReadOnlyList<Claim> Items, int TotalCount)> SearchAsync(ClaimQuery query, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<ClaimDocument?> GetDocumentByIdAsync(Guid id, CancellationToken ct = default) => throw new NotSupportedException();
        public Task AddAsync(Claim claim, CancellationToken ct = default) => throw new NotSupportedException();
        public Task AddStatusHistoryAsync(ClaimStatusHistory history, CancellationToken ct = default) { HistoryCount++; return Task.CompletedTask; }
        public Task AddDocumentAsync(ClaimDocument doc, CancellationToken ct = default) => throw new NotSupportedException();
        public Task SaveChangesAsync(CancellationToken ct = default) { Saves++; return Task.CompletedTask; }
    }
}
