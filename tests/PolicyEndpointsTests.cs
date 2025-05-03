using System.Net;
using System.Net.Http.Json;
using application.DTOs;
using domain.Models.Policy;
using domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace tests;

public class PolicyEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PolicyEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteWorkflow_Success()
    {
        // Create Policy
        var createResponse = await CreatePolicy();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var policy = await createResponse.Content.ReadFromJsonAsync<Policy>();
        Assert.NotNull(policy);

        // Get Policy
        var getResponse = await _client.GetAsync($"/api/Policy/{policy.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // Calculate Premium
        var premiumRequest = new PremiumRequestDTO
        {
            PolicyId = policy.Id,
            MarketValue = 10000
        };
        var premiumResponse = await _client.PostAsJsonAsync("/api/Quote/request-quote", premiumRequest);
        Assert.Equal(HttpStatusCode.OK, premiumResponse.StatusCode);
        var premium = await premiumResponse.Content.ReadFromJsonAsync<PremiumResponseDTO>();
        Assert.NotNull(premium);

        // Update Policy
        var updateDto = new UpdatePolicyDTO
        {
            PolicyName = "Updated Policy"
        };
        var updateResponse = await _client.PatchAsJsonAsync($"/api/Policy/{policy.Id}", updateDto);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Delete Policy
        var deleteResponse = await _client.DeleteAsync($"/api/Policy/{policy.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    private async Task<HttpResponseMessage> CreatePolicy()
    {
        var createDto = new CreatePolicyDTO
        {
            PolicyName = "Test Policy",
            Components = new List<CreatePolicyComponentDTO>
            {
                new()
                {
                    Sequence = 1,
                    Name = "Premium Base",
                    Operation = OperationType.Add,
                    FlatValue = 300,
                    PercentageValue = 0
                },
                new()
                {
                    Sequence = 2,
                    Name = "Market Value Premium",
                    Operation = OperationType.Add,
                    FlatValue = 0,
                    PercentageValue = 10
                }
            }
        };

        return await _client.PostAsJsonAsync("/api/Policy", createDto);
    }
}
