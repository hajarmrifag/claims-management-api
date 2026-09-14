using System.Net;
using System.Net.Http.Json;

namespace Claims.IntegrationTests;

public class AuthenticationRateLimitTests :
    IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthenticationRateLimitTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_AfterRepeatedFailures_ShouldBeRateLimited()
    {
        HttpResponseMessage? response = null;

        for (var attempt = 0; attempt < 6; attempt++)
        {
            response = await _client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    email = "missing@example.com",
                    password = "IncorrectPassword123!"
                });
        }

        Assert.NotNull(response);
        Assert.Equal(
            HttpStatusCode.TooManyRequests,
            response.StatusCode);
    }
}
