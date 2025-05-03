using application.DTOs;
using application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuoteController : ControllerBase
{
    private readonly IPremiumCalculatorService _calculatorService;

    public QuoteController(IPremiumCalculatorService calculatorService)
    {
        _calculatorService = calculatorService;
    }

    [HttpPost("request-quote")]
    public async Task<ActionResult<PremiumResponseDTO>> RequestQuote([FromBody] PremiumRequestDTO request)
    {
        try
        {
            var result = await _calculatorService.CalculatePremiumAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while calculating the premium");
        }
    }
}