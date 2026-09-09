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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClaimResponse>> GetClaimById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var claim = await _claimService.GetByIdAsync(
            id,
            cancellationToken);

        if (claim is null)
        {
            return NotFound();
        }

        return Ok(claim);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ClaimResponse>>> SearchClaims(
        [FromQuery] ClaimQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _claimService.SearchAsync(
            query,
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ClaimResponse>> UpdateClaimStatus(
        Guid id,
        UpdateClaimStatusRequest request,
        CancellationToken cancellationToken)
    {
        var claim = await _claimService.UpdateStatusAsync(
            id,
            request,
            cancellationToken);

        if (claim is null)
        {
            return NotFound();
        }

        return Ok(claim);
    }
}
