using System.Net;
using System.Net.Http.Json;

namespace Claims.IntegrationTests;

public class RoleAuthorizationTests :
    IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RoleAuthorizationTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Adjuster_ShouldNotBeAllowedToChangeClaimStatus()
    {
        await TestAuthHelper.AuthenticateAsync(_client);

        var response = await _client.PatchAsJsonAsync(
            $"/api/claims/{Guid.NewGuid()}/status",
            new
            {
                status = "Approved"
            });

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}
