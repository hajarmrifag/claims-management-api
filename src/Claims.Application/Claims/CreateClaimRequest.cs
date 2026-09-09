namespace Claims.Application.Claims;

public record CreateClaimRequest(
    string ClaimNumber,
    Guid CustomerId,
    Guid PolicyId,
    string Description,
    decimal Amount
);
