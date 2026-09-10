using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Policies;

public record CreatePolicyRequest(
    [param: Required]
    [param: StringLength(50, MinimumLength = 1)]
    string PolicyNumber,

    Guid CustomerId,

    [param: Required]
    [param: StringLength(100, MinimumLength = 1)]
    string PolicyType,

    [param: Range(0.01, double.MaxValue)]
    decimal CoverageAmount,

    DateTime StartDate,

    DateTime EndDate
);
