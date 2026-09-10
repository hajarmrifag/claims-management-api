using System.Net;
using System.Net.Http.Json;
using Claims.Application.Auth;

namespace Claims.IntegrationTests;

public class AuthApiTests :
    IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterAndLogin_ShouldReturnJwtTokens()
    {
        var email = $"user-{Guid.NewGuid()}@example.com";
        const string password = "StrongPassword123!";

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, password));

        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var registeredUser =
            await registerResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(registeredUser);
        Assert.False(
            string.IsNullOrWhiteSpace(registeredUser.Token));
        Assert.Equal("Adjuster", registeredUser.Role);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(email, password));

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var loggedInUser =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loggedInUser);
        Assert.False(
            string.IsNullOrWhiteSpace(loggedInUser.Token));
        Assert.Equal(email, loggedInUser.Email);
    }
}
