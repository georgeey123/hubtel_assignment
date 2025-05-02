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
    public async Task<IActionResult> RequestQuote([FromBody] PremiumRequestDTO request)
    {
        if (request.MarketValue <= 0)
            return BadRequest("Market value must be greater than zero.");

        var result = await _calculatorService.CalculatePremiumAsync(request);
        if (result == null)
            return NotFound("Policy not found.");

        return Ok(result);
    }
}