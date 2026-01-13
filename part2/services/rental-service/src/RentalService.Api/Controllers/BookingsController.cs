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

        // After creation, the canonical read endpoint is GET /rentals/{rentalId}
        return Created($"/rentals/{result.RentalId}", result);
    }
}
