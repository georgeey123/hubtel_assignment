using application.DTOs;
using application.Interfaces;
using domain.Enums;

namespace application.Services;

public class PremiumCalculatorService : IPremiumCalculatorService
{
    private readonly IPolicyRepository _policyRepository;

    public PremiumCalculatorService(IPolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }

    public async Task<PremiumResponseDTO?> CalculatePremiumAsync(PremiumRequestDTO request)
    {
        var policy = await _policyRepository.GetByIdAsync(request.PolicyId);
        if (policy == null || policy.Components == null || !policy.Components.Any())
            return null;

        var orderedComponents = policy.Components.OrderBy(c => c.Sequence);
        decimal premium = 0;

        foreach (var component in orderedComponents)
        {
            decimal value = component.FlatValue;

            if (component.Name.Contains("Market Value", StringComparison.OrdinalIgnoreCase) &&
                component.PercentageValue > 0)
            {
                value += request.MarketValue * (component.PercentageValue / 100);
            }

            if (component.Operation == OperationType.Add)
                premium += value;
            else if (component.Operation == OperationType.Subtract)
                premium -= value;
        }

        return new PremiumResponseDTO()
        {
            PolicyId = policy.Id,
            PolicyName = policy.PolicyName,
            Premium = Math.Round(premium, 2)
        };        
    }
}