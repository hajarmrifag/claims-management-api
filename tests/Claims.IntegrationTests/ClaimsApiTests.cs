using System.Net;
using System.Net.Http.Json;
using Claims.Application.Claims;
using Claims.Application.Customers;
using Claims.Application.Policies;

namespace Claims.IntegrationTests;

public class ClaimsApiTests :
    IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClaimsApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ClaimWorkflow_ShouldCreateAndRetrieveClaim()
    {
        var customerRequest = new
        {
            firstName = "Test",
            lastName = "Customer",
            email = $"test-{Guid.NewGuid()}@example.com",
            phoneNumber = "123456789"
        };

        var customerResponse = await _client.PostAsJsonAsync(
            "/api/customers",
            customerRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            customerResponse.StatusCode);

        var customer = await customerResponse.Content
            .ReadFromJsonAsync<CustomerResponse>();

        Assert.NotNull(customer);

        var policyRequest = new
        {
            policyNumber = $"POL-{Guid.NewGuid()}",
            customerId = customer.Id,
            policyType = "Health",
            coverageAmount = 100000m,
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddYears(1)
        };

        var policyResponse = await _client.PostAsJsonAsync(
            "/api/policies",
            policyRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            policyResponse.StatusCode);

        var policy = await policyResponse.Content
            .ReadFromJsonAsync<PolicyResponse>();

        Assert.NotNull(policy);

        var claimRequest = new
        {
            claimNumber = $"CLM-{Guid.NewGuid()}",
            customerId = customer.Id,
            policyId = policy.Id,
            description = "Integration test claim",
            amount = 2500m
        };

        var createClaimResponse = await _client.PostAsJsonAsync(
            "/api/claims",
            claimRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            createClaimResponse.StatusCode);

        var claim = await createClaimResponse.Content
            .ReadFromJsonAsync<ClaimResponse>();

        Assert.NotNull(claim);
        Assert.Equal("Submitted", claim.Status);

        var getClaimResponse = await _client.GetAsync(
            $"/api/claims/{claim.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            getClaimResponse.StatusCode);

        var retrievedClaim = await getClaimResponse.Content
            .ReadFromJsonAsync<ClaimResponse>();

        Assert.NotNull(retrievedClaim);
        Assert.Equal(claim.Id, retrievedClaim.Id);
        Assert.Equal(claim.ClaimNumber, retrievedClaim.ClaimNumber);
    }
}
