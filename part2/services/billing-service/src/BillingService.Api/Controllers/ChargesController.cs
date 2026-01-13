using Microsoft.AspNetCore.Mvc;
using BillingService.Application.Dtos;
using BillingService.Application.UseCases;

namespace BillingService.Api.Controllers;

[ApiController]
[Route("charges")]
public sealed class ChargesController : ControllerBase
{
    private readonly CreateChargeFromRentalUseCase _useCase;

    public ChargesController(CreateChargeFromRentalUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpPost("from-rental")]
    public async Task<ActionResult<CreateChargeFromRentalResponseDto>> CreateFromRental(
        [FromBody] CreateChargeFromRentalRequestDto req,
        CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(req, ct);
        return Ok(result);
    }
}
