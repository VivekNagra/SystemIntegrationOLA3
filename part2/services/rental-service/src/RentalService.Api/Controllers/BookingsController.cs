using Microsoft.AspNetCore.Mvc;
using RentalService.Application.Dtos;
using RentalService.Application.UseCases;

namespace RentalService.Api.Controllers;

[ApiController]
[Route("bookings")]
public sealed class BookingsController : ControllerBase
{
    private readonly CreateBookingUseCase _useCase;

    public BookingsController(CreateBookingUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpPost]
    public async Task<ActionResult<CreateBookingResponseDto>> Create(
        [FromBody] CreateBookingRequestDto req,
        CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { rentalId = result.RentalId }, result);
    }

    // For demo convenience (no separate query model)
    [HttpGet("{rentalId:guid}")]
    public ActionResult<object> GetById(Guid rentalId)
    {
        // We keep this endpoint minimal for now.
        // Query side (read models) can be added later if needed.
        return Ok(new { rentalId });
    }
}
