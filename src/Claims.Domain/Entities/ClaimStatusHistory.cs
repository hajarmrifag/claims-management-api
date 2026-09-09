using Claims.Domain.Enums;

namespace Claims.Domain.Entities;

public class ClaimStatusHistory
{
    public Guid Id { get; private set; }

    public Guid ClaimId { get; private set; }

    public ClaimStatus PreviousStatus { get; private set; }

    public ClaimStatus NewStatus { get; private set; }

    public DateTime ChangedAt { get; private set; }

    private ClaimStatusHistory()
    {
    }

    public ClaimStatusHistory(
        Guid claimId,
        ClaimStatus previousStatus,
        ClaimStatus newStatus)
    {
        Id = Guid.NewGuid();
        ClaimId = claimId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ChangedAt = DateTime.UtcNow;
    }
}
