using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Claims;

public record CreateClaimRequest(
    [param: Required]
    [param: StringLength(50, MinimumLength = 1)]
    string ClaimNumber,

    Guid CustomerId,

    Guid PolicyId,

    [param: Required]
    [param: StringLength(2000, MinimumLength = 1)]
    string Description,

    [param: Range(0.01, double.MaxValue)]
    decimal Amount
);
