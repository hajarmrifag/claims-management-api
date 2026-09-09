namespace Claims.Application.Policies;

public record PolicyResponse(
    Guid Id,
    string PolicyNumber,
    Guid CustomerId,
    string PolicyType,
    decimal CoverageAmount,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive
);
