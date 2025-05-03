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
    private readonly Mock<IPolicyRepository> _mockRepository;
    private readonly PremiumCalculatorService _service;

    public PremiumCalculatorServiceTests()
    {
        _mockRepository = new Mock<IPolicyRepository>();
        _service = new PremiumCalculatorService(_mockRepository.Object);
    }

    [Fact]
    public async Task CalculatePremium_WithFlatValues_CalculatesCorrectly()
    {
        // Arrange
        var policy = CreatePolicyWithFlatValues();
        var request = new PremiumRequestDTO { PolicyId = 1, MarketValue = 0 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(policy);

        // Act
        var result = await _service.CalculatePremiumAsync(request);

        // Assert
        Assert.Equal(400, result.Premium); // 300 + 100
        Assert.Equal(2, result.CalculationDetails.Count);
    }

    [Fact]
    public async Task CalculatePremium_WithMarketValuePercentage_CalculatesCorrectly()
    {
        // Arrange
        var policy = CreatePolicyWithMarketValueComponent();
        var request = new PremiumRequestDTO { PolicyId = 1, MarketValue = 10000 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(policy);

        // Act
        var result = await _service.CalculatePremiumAsync(request);

        // Assert
        Assert.Equal(1300, result.Premium); // 300 + 1000 (10% of 10000)
        Assert.Equal(2, result.CalculationDetails.Count);
    }

    [Fact]
    public async Task CalculatePremium_WithDiscount_SubtractsCorrectly()
    {
        // Arrange
        var policy = CreatePolicyWithDiscount();
        var request = new PremiumRequestDTO { PolicyId = 1, MarketValue = 0 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(policy);

        // Act
        var result = await _service.CalculatePremiumAsync(request);

        // Assert
        Assert.Equal(250, result.Premium); // 300 - 50
        Assert.Equal(2, result.CalculationDetails.Count);
    }

    [Fact]
    public async Task CalculatePremium_WithInvalidPolicy_ThrowsException()
    {
        // Arrange
        var request = new PremiumRequestDTO { PolicyId = 999, MarketValue = 0 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Policy)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CalculatePremiumAsync(request));
    }

    [Fact]
    public async Task CalculatePremium_ExecutesComponentsInSequence()
    {
        // Arrange
        var policy = CreateComplexPolicy();
        var request = new PremiumRequestDTO { PolicyId = 1, MarketValue = 10000 };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(policy);

        // Act
        var result = await _service.CalculatePremiumAsync(request);

        // Assert
        var details = result.CalculationDetails.OrderBy(d => d.Sequence).ToList();
        Assert.Equal(4, details.Count);
        Assert.Equal("Premium Base", details[0].ComponentName);
        Assert.Equal("Extra Perils", details[1].ComponentName);
        Assert.Equal("Market Value Premium", details[2].ComponentName);
        Assert.Equal("Promo Discount", details[3].ComponentName);
    }

    private static Policy CreatePolicyWithFlatValues()
    {
        return new Policy
        {
            Id = 1,
            PolicyName = "Flat Value Policy",
            Components = new List<PolicyComponent>
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
                    Name = "Extra Perils",
                    Operation = OperationType.Add,
                    FlatValue = 100,
                    PercentageValue = 0
                }
            }
        };
    }

    private static Policy CreatePolicyWithMarketValueComponent()
    {
        return new Policy
        {
            Id = 1,
            PolicyName = "Market Value Policy",
            Components = new List<PolicyComponent>
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
    }

    private static Policy CreatePolicyWithDiscount()
    {
        return new Policy
        {
            Id = 1,
            PolicyName = "Discount Policy",
            Components = new List<PolicyComponent>
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
                    Name = "Promo Discount",
                    Operation = OperationType.Subtract,
                    FlatValue = 50,
                    PercentageValue = 0
                }
            }
        };
    }

    private static Policy CreateComplexPolicy()
    {
        return new Policy
        {
            Id = 1,
            PolicyName = "Complex Policy",
            Components = new List<PolicyComponent>
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
                    Name = "Extra Perils",
                    Operation = OperationType.Add,
                    FlatValue = 100,
                    PercentageValue = 0
                },
                new()
                {
                    Sequence = 3,
                    Name = "Market Value Premium",
                    Operation = OperationType.Add,
                    FlatValue = 0,
                    PercentageValue = 10
                },
                new()
                {
                    Sequence = 4,
                    Name = "Promo Discount",
                    Operation = OperationType.Subtract,
                    FlatValue = 50,
                    PercentageValue = 0
                }
            }
        };
    }
}

