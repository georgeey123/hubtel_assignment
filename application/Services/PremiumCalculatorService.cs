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

    public async Task<PremiumResponseDTO> CalculatePremiumAsync(PremiumRequestDTO request)
    {
        var policy = await _policyRepository.GetByIdAsync(request.PolicyId);
        if (policy == null)
            throw new ArgumentException("Policy not found", nameof(request.PolicyId));

        var response = new PremiumResponseDTO
        {
            PolicyId = policy.Id,
            PolicyName = policy.PolicyName,
            CalculationDetails = new List<CalculationDetail>()
        };

        decimal runningTotal = 0;

        foreach (var component in policy.Components.OrderBy(c => c.Sequence))
        {
            decimal componentAmount = 0;

            // Calculate flat value
            if (component.FlatValue > 0)
            {
                componentAmount += component.FlatValue;
            }

            // Calculate percentage-based value
            if (component.PercentageValue > 0)
            {
                if (component.Name == "Market Value Premium")
                {
                    // Apply percentage to market value
                    componentAmount += (request.MarketValue * component.PercentageValue) / 100;
                }
                else
                {
                    // Apply percentage to running total
                    componentAmount += (runningTotal * component.PercentageValue) / 100;
                }
            }

            // Apply operation
            if (component.Operation == OperationType.Add)
            {
                runningTotal += componentAmount;
            }
            else if (component.Operation == OperationType.Subtract)
            {
                runningTotal -= componentAmount;
            }

            // Record calculation step
            // response.CalculationDetails.Add(new CalculationDetail
            // {
            //     Sequence = component.Sequence,
            //     ComponentName = component.Name,
            //     Operation = component.Operation.ToString(),
            //     Amount = componentAmount,
            //     RunningTotal = runningTotal
            // });
        }

        response.Premium = runningTotal;
        return response;
    }
}
