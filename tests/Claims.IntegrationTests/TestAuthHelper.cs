using System.Net.Http.Json;
using System.Net.Http.Headers;
using Claims.Application.Auth;

namespace Claims.IntegrationTests;

public static class TestAuthHelper
{
    public static async Task AuthenticateAsync(HttpClient client)
    {
        var email = $"test-{Guid.NewGuid()}@example.com";
        const string password = "StrongPassword123!";

        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, password));

        response.EnsureSuccessStatusCode();

        var auth = await response.Content
            .ReadFromJsonAsync<LoginResponse>();

        if (auth is null)
        {
            throw new InvalidOperationException(
                "Authentication response was empty.");
        }

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                auth.Token);
    }
}
