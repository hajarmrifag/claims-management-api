using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Claims;

public record UpdateClaimStatusRequest(
    [property: Required]
    [property: StringLength(30, MinimumLength = 1)]
    string Status
);
