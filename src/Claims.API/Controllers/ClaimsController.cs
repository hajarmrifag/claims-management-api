using Claims.Application.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly ClaimService _claimService;

    public ClaimsController(ClaimService claimService)
    {
        _claimService = claimService;
    }

    [HttpPost]
    public async Task<ActionResult<ClaimResponse>> CreateClaim(
        CreateClaimRequest request,
        CancellationToken cancellationToken)
    {
        var claim = await _claimService.CreateAsync(
            request,
            cancellationToken);

        return Created(
            $"/api/claims/{claim.Id}",
            claim);
    }
}
