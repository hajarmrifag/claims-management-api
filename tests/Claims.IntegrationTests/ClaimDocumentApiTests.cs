using System.Net;
using System.Net.Http.Json;
using System.Text;
using Claims.Application.Claims;
using Claims.Application.Customers;
using Claims.Application.Documents;
using Claims.Application.Policies;

namespace Claims.IntegrationTests;

public class ClaimDocumentApiTests :
    IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClaimDocumentApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UploadAndDownloadDocument_ShouldWork()
    {
        await TestAuthHelper.AuthenticateAsync(_client);

        var customerResponse = await _client.PostAsJsonAsync(
            "/api/customers",
            new
            {
                firstName = "Document",
                lastName = "Tester",
                email = $"document-{Guid.NewGuid()}@example.com",
                phoneNumber = "123456789"
            });

        customerResponse.EnsureSuccessStatusCode();

        var customer = await customerResponse.Content
            .ReadFromJsonAsync<CustomerResponse>();

        Assert.NotNull(customer);

        var policyResponse = await _client.PostAsJsonAsync(
            "/api/policies",
            new
            {
                policyNumber = $"POL-{Guid.NewGuid()}",
                customerId = customer.Id,
                policyType = "Health",
                coverageAmount = 100000m,
                startDate = DateTime.UtcNow,
                endDate = DateTime.UtcNow.AddYears(1)
            });

        policyResponse.EnsureSuccessStatusCode();

        var policy = await policyResponse.Content
            .ReadFromJsonAsync<PolicyResponse>();

        Assert.NotNull(policy);

        var claimResponse = await _client.PostAsJsonAsync(
            "/api/claims",
            new
            {
                claimNumber = $"CLM-{Guid.NewGuid()}",
                customerId = customer.Id,
                policyId = policy.Id,
                description = "Document upload integration test",
                amount = 2500m
            });

        claimResponse.EnsureSuccessStatusCode();

        var claim = await claimResponse.Content
            .ReadFromJsonAsync<ClaimResponse>();

        Assert.NotNull(claim);

        const string expectedContent = "fake pdf content";

        using var content = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(
            Encoding.UTF8.GetBytes(expectedContent));

        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(
                "application/pdf");

        content.Add(
            fileContent,
            "file",
            "claim-document.pdf");

        var uploadResponse = await _client.PostAsync(
            $"/api/claims/{claim.Id}/documents",
            content);

        Assert.Equal(
            HttpStatusCode.Created,
            uploadResponse.StatusCode);

        var document = await uploadResponse.Content
            .ReadFromJsonAsync<ClaimDocumentResponse>();

        Assert.NotNull(document);

        var downloadResponse = await _client.GetAsync(
            $"/api/claims/{claim.Id}/documents/{document.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            downloadResponse.StatusCode);

        var downloadedContent =
            await downloadResponse.Content.ReadAsStringAsync();

        Assert.Equal(expectedContent, downloadedContent);
    }
}
