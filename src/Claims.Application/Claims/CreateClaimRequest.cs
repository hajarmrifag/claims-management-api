using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Claims;

public record CreateClaimRequest(
    [property: Required]
    [property: StringLength(50, MinimumLength = 1)]
    string ClaimNumber,

    Guid CustomerId,

    Guid PolicyId,

    [property: Required]
    [property: StringLength(2000, MinimumLength = 1)]
    string Description,

    [property: Range(0.01, double.MaxValue)]
    decimal Amount
);
