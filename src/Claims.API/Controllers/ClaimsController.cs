using Claims.Application.Claims;
using Claims.Application.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers;

[ApiController]
[Authorize]
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
    [Authorize(Roles = "Manager,Admin")]
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

    [HttpPost("{id:guid}/documents")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<ClaimDocumentResponse>> UploadDocument(
        Guid id,
        IFormFile file,
        [FromServices] ClaimDocumentService documentService,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest("File cannot be empty.");
        }

        var allowedContentTypes = new[]
        {
            "application/pdf",
            "image/jpeg",
            "image/png"
        };

        if (!allowedContentTypes.Contains(
                file.ContentType,
                StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(
                "Only PDF, JPEG and PNG files are allowed.");
        }

        await using var stream = file.OpenReadStream();

        var response = await documentService.UploadAsync(
            id,
            stream,
            file.FileName,
            file.ContentType,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }


    [HttpGet("{claimId:guid}/documents/{documentId:guid}")]
    public async Task<IActionResult> DownloadDocument(
        Guid claimId,
        Guid documentId,
        [FromServices] ClaimDocumentService documentService,
        CancellationToken cancellationToken)
    {
        var (stream, fileName) = await documentService.DownloadAsync(
            claimId,
            documentId,
            cancellationToken);

        return File(
            stream,
            "application/octet-stream",
            fileName);
    }

}
