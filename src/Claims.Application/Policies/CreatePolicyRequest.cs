namespace Claims.Application.Policies;

public record CreatePolicyRequest(
    string PolicyNumber,
    Guid CustomerId,
    string PolicyType,
    decimal CoverageAmount,
    DateTime StartDate,
    DateTime EndDate
);
