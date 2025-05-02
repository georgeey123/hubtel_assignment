using Xunit;
using Moq;
using domain.Models.Policy;
using domain.Enums;
using application.DTOs;
using application.Interfaces;
using application.Services;

namespace tests;

public class PremiumCalculatorServiceTests
{
    private readonly Mock<IPolicyRepository> _mockRepo;
    private readonly PremiumCalculatorService _service;

    public PremiumCalculatorServiceTests()
    {
        _mockRepo = new Mock<IPolicyRepository>();
        _service = new PremiumCalculatorService(_mockRepo.Object);
    }

    [Fact]
    public async Task CalculatePremiumAsync_ReturnsCorrectPremium()
    {
        // Arrange
        var policy = new Policy
        {
            Id = 1,
            PolicyName = "Test Policy",
            Components = new List<PolicyComponent>
            {
                new PolicyComponent { Sequence = 1, Name = "Base", Operation = OperationType.Add, FlatValue = 200, PercentageValue = 0 },
                new PolicyComponent { Sequence = 2, Name = "Market Value Premium", Operation = OperationType.Add, FlatValue = 0, PercentageValue = 10 },
                new PolicyComponent { Sequence = 3, Name = "Discount", Operation = OperationType.Subtract, FlatValue = 50, PercentageValue = 0 }
            }
        };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(policy);

        var request = new PremiumRequestDTO { PolicyId = 1, MarketValue = 10000 };

        // Act
        var result = await _service.CalculatePremiumAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.PolicyId);
        Assert.Equal("Test Policy", result.PolicyName);
        Assert.Equal(1150.00m, result.Premium); // 200 + (10000*0.10) - 50
    }

    [Fact]
    public async Task CalculatePremiumAsync_ReturnsNull_IfPolicyNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Policy?)null);

        var result = await _service.CalculatePremiumAsync(new PremiumRequestDTO { PolicyId = 99, MarketValue = 10000 });

        Assert.Null(result);
    }
}
