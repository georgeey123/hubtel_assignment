using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using web.Controllers;
using application.Interfaces;
using domain.Models.Policy;
using domain.Enums;
using application.DTOs;


namespace tests;

public class PolicyControllerTests
{
    private readonly Mock<IPolicyRepository> _mockRepository;
    private readonly Mock<IPremiumCalculatorService> _mockPremiumCalculator;
    private readonly PolicyController _controller;

    public PolicyControllerTests()
    {
        _mockRepository = new Mock<IPolicyRepository>();
        _mockPremiumCalculator = new Mock<IPremiumCalculatorService>();
        _controller = new PolicyController(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllPolicies_ReturnsOkResult()
    {
        // Arrange
        var expectedPolicies = new List<Policy> { CreateSamplePolicy() };
        _mockRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedPolicies);

        // Act
        var result = await _controller.GetAllPolicies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPolicies = Assert.IsType<List<Policy>>(okResult.Value);
        Assert.Single(returnedPolicies);
    }

    [Fact]
    public async Task GetPolicyById_WithValidId_ReturnsPolicy()
    {
        // Arrange
        var policy = CreateSamplePolicy();
        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(policy);

        // Act
        var result = await _controller.GetPolicyById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPolicy = Assert.IsType<Policy>(okResult.Value);
        Assert.Equal(policy.Id, returnedPolicy.Id);
    }

    [Fact]
    public async Task GetPolicyById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Policy)null);

        // Act
        var result = await _controller.GetPolicyById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreatePolicy_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreatePolicyDTO
        {
            PolicyName = "Test Policy",
            Components = new List<CreatePolicyComponentDTO>
            {
                new() { 
                    Sequence = 1,
                    Name = "Base Premium",
                    Operation = OperationType.Add,
                    FlatValue = 100,
                    PercentageValue = 0
                }
            }
        };

        var createdPolicy = CreateSamplePolicy();
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Policy>()))
            .ReturnsAsync(createdPolicy);

        // Act
        var result = await _controller.CreatePolicy(createDto);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(PolicyController.GetPolicyById), createdAtResult.ActionName);
    }

    [Fact]
    public async Task PatchPolicy_WithValidUpdate_ReturnsOkResult()
    {
        // Arrange
        var existingPolicy = CreateSamplePolicy();
        var updateDto = new UpdatePolicyDTO
        {
            PolicyName = "Updated Policy",
            Components = new List<UpdatePolicyComponentDTO>
            {
                new() {
                    Sequence = 1,
                    Name = "Updated Component"
                }
            }
        };

        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingPolicy);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Policy>()))
            .ReturnsAsync(existingPolicy);

        // Act
        var result = await _controller.PatchPolicy(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPolicy = Assert.IsType<Policy>(okResult.Value);
        Assert.Equal("Updated Policy", returnedPolicy.PolicyName);
    }

    [Fact]
    public async Task DeletePolicy_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeletePolicy(1);

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }

    private static Policy CreateSamplePolicy()
    {
        return new Policy
        {
            Id = 1,
            PolicyName = "Test Policy",
            Components = new List<PolicyComponent>
            {
                new()
                {
                    Sequence = 1,
                    Name = "Base Premium",
                    Operation = OperationType.Add,
                    FlatValue = 100,
                    PercentageValue = 0
                }
            }
        };
    }
}
