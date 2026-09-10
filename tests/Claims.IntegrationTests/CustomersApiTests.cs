using System.Net;
using System.Net.Http.Json;

namespace Claims.IntegrationTests;

public class CustomersApiTests :
    IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CustomersApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnCreated()
    {
        await TestAuthHelper.AuthenticateAsync(_client);

        var request = new
        {
            firstName = "Hajar",
            lastName = "Test",
            email = "hajar.test@example.com",
            phoneNumber = "123456789"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/customers",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }
}
