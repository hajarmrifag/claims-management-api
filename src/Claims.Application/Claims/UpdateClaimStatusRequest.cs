using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Claims;

public record UpdateClaimStatusRequest(
    [param: Required]
    [param: StringLength(30, MinimumLength = 1)]
    string Status
);
