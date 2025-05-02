using application.DTOs;

namespace application.Interfaces;

public interface IPremiumCalculatorService
{
    Task<PremiumResponseDTO> CalculatePremiumAsync (PremiumRequestDTO request);
}