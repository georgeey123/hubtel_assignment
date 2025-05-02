using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using web.Controllers;
using application.Interfaces;
using domain.Models.Policy;

namespace tests;

public class PolicyControllerTests
{
    private readonly Mock<IPolicyRepository> _mockRepo;
    private readonly PolicyController _controller;

    public PolicyControllerTests()
    {
        _mockRepo = new Mock<IPolicyRepository>();
        _controller = new PolicyController(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllPolicies_ReturnsOk_WithPolicies()
    {
        // Arrange
        var policies = new List<Policy> { new Policy { Id = 1 }, new Policy { Id = 2 } };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(policies);

        // Act
        var result = await _controller.GetAllPolicies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPolicies = Assert.IsType<List<Policy>>(okResult.Value);
        Assert.Equal(2, returnedPolicies.Count);
    }

    [Fact]
    public async Task GetAllPolicies_ReturnsOk_WithEmptyList()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Policy>());

        var result = await _controller.GetAllPolicies();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPolicies = Assert.IsType<List<Policy>>(okResult.Value);
        Assert.Empty(returnedPolicies);
    }

    [Fact]
    public async Task GetPolicyById_ReturnsOk_WhenFound()
    {
        var policy = new Policy { Id = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(policy);

        var result = await _controller.GetPolicyById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<Policy>(okResult.Value);
        Assert.Equal(1, returned.Id);
    }

    [Fact]
    public async Task GetPolicyById_ReturnsNotFound_WhenNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Policy?)null);

        var result = await _controller.GetPolicyById(1);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreatePolicy_ReturnsCreatedAtAction()
    {
        var newPolicy = new Policy { Id = 1 };
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Policy>())).ReturnsAsync(newPolicy);

        var result = await _controller.CreatePolicy(newPolicy);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var value = Assert.IsType<Policy>(created.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task UpdatePolicy_ReturnsOk_WhenSuccessful()
    {
        var policy = new Policy { Id = 1 };
        _mockRepo.Setup(r => r.UpdateAsync(policy)).ReturnsAsync(policy);

        var result = await _controller.UpdatePolicy(1, policy);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var updated = Assert.IsType<Policy>(ok.Value);
        Assert.Equal(1, updated.Id);
    }

    [Fact]
    public async Task UpdatePolicy_ReturnsBadRequest_WhenIdMismatch()
    {
        var policy = new Policy { Id = 2 };

        var result = await _controller.UpdatePolicy(1, policy);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Id does not match", badRequest.Value);
    }

    [Fact]
    public async Task UpdatePolicy_ReturnsNotFound_WhenPolicyNotFound()
    {
        var policy = new Policy { Id = 1 };
        _mockRepo.Setup(r => r.UpdateAsync(policy)).ReturnsAsync((Policy?)null);

        var result = await _controller.UpdatePolicy(1, policy);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeletePolicy_ReturnsNoContent_WhenSuccessful()
    {
        _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeletePolicy(1);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task DeletePolicy_ReturnsNotFound_WhenFailed()
    {
        _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(false);

        var result = await _controller.DeletePolicy(1);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}
