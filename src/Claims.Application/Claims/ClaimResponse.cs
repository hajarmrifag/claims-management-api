namespace Claims.Application.Claims;

public record ClaimResponse(
    Guid Id,
    string ClaimNumber,
    Guid CustomerId,
    Guid PolicyId,
    string Description,
    decimal Amount,
    string Status,
    DateTime SubmittedAt
);
