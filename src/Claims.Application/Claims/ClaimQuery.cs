namespace Claims.Application.Claims;

public record ClaimQuery(
    string? Status,
    DateTime? FromDate,
    DateTime? ToDate,
    int Page = 1,
    int PageSize = 20
);
