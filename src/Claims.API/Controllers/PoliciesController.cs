using Claims.Application.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly PolicyService _policyService;

    public PoliciesController(PolicyService policyService)
    {
        _policyService = policyService;
    }

    [HttpPost]
    public async Task<ActionResult<PolicyResponse>> CreatePolicy(
        CreatePolicyRequest request,
        CancellationToken cancellationToken)
    {
        var policy = await _policyService.CreateAsync(
            request,
            cancellationToken);

        return Created(
            $"/api/policies/{policy.Id}",
            policy);
    }
}
