using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Policies;

public record CreatePolicyRequest(
    [property: Required]
    [property: StringLength(50, MinimumLength = 1)]
    string PolicyNumber,

    Guid CustomerId,

    [property: Required]
    [property: StringLength(100, MinimumLength = 1)]
    string PolicyType,

    [property: Range(0.01, double.MaxValue)]
    decimal CoverageAmount,

    DateTime StartDate,

    DateTime EndDate
);
