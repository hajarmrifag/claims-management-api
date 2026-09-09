using Claims.Domain.Enums;

namespace Claims.Domain.Entities;

public class Claim
{
    public Guid Id { get; private set; }

    public string ClaimNumber { get; private set; } = string.Empty;

    public Guid CustomerId { get; private set; }

    public Guid PolicyId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public decimal Amount { get; private set; }

    public ClaimStatus Status { get; private set; }

    public DateTime SubmittedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    private Claim()
    {
    }

    public Claim(
        string claimNumber,
        Guid customerId,
        Guid policyId,
        string description,
        decimal amount)
    {
        Id = Guid.NewGuid();
        ClaimNumber = claimNumber;
        CustomerId = customerId;
        PolicyId = policyId;
        Description = description;
        Amount = amount;
        Status = ClaimStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ClaimStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
