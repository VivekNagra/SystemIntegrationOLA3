using Microsoft.AspNetCore.Mvc;
using RentalService.Application.Dtos;
using RentalService.Application.UseCases;

namespace RentalService.Api.Controllers;

[ApiController]
[Route("rentals")]
public sealed class RentalsController : ControllerBase
{
    private readonly GetRentalUseCase _get;
    private readonly StartRentalUseCase _start;
    private readonly ReturnRentalUseCase _return;

    public RentalsController(GetRentalUseCase get, StartRentalUseCase start, ReturnRentalUseCase @return)
    {
        _get = get;
        _start = start;
        _return = @return;
    }

    [HttpGet("{rentalId:guid}")]
    public async Task<ActionResult<RentalDetailsResponseDto>> GetById(Guid rentalId, CancellationToken ct)
    {
        var result = await _get.ExecuteAsync(rentalId, ct);
        return Ok(result);
    }

    [HttpPost("{rentalId:guid}/start")]
    public async Task<ActionResult<StartRentalResponseDto>> Start(Guid rentalId, CancellationToken ct)
    {
        var result = await _start.ExecuteAsync(rentalId, ct);
        return Ok(result);
    }

    [HttpPost("{rentalId:guid}/return")]
    public async Task<ActionResult<ReturnRentalResponseDto>> Return(
        Guid rentalId,
        [FromBody] ReturnRentalRequestDto req,
        CancellationToken ct)
    {
        var result = await _return.ExecuteAsync(rentalId, req, ct);
        return Ok(result);
    }
}
