namespace Claims.Domain.Entities;

public class Policy
{
    public Guid Id { get; private set; }

    public string PolicyNumber { get; private set; } = string.Empty;

    public Guid CustomerId { get; private set; }

    public string PolicyType { get; private set; } = string.Empty;

    public decimal CoverageAmount { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public bool IsActive { get; private set; }

    private Policy()
    {
    }

    public Policy(
        string policyNumber,
        Guid customerId,
        string policyType,
        decimal coverageAmount,
        DateTime startDate,
        DateTime endDate)
    {
        Id = Guid.NewGuid();
        PolicyNumber = policyNumber;
        CustomerId = customerId;
        PolicyType = policyType;
        CoverageAmount = coverageAmount;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
