using RentalService.Application.Dtos;
using RentalService.Application.Ports;
using RentalService.Domain.Aggregates;
using RentalService.Domain.ValueObjects;

namespace RentalService.Application.UseCases;

public sealed class CreateBookingUseCase
{
    private readonly IRentalRepository _repo;

    public CreateBookingUseCase(IRentalRepository repo)
    {
        _repo = repo;
    }

    public async Task<CreateBookingResponseDto> ExecuteAsync(CreateBookingRequestDto req, CancellationToken ct)
    {
        var trailerId = new TrailerId(req.LocationId, req.TrailerNumber);
        var trailerKey = trailerId.ToString();

        // Availability rule (simplified for iteration 1)
        var hasActive = await _repo.HasActiveRentalForTrailerAsync(trailerKey, ct);
        if (hasActive)
        {
            // In a real implementation we'd return a structured problem response from the API layer.
            throw new InvalidOperationException($"Trailer {trailerKey} is not available for booking.");
        }

        var rental = Rental.Book(trailerId, req.CustomerId, req.DesiredStartTime);

        if (req.InsuranceSelected)
            rental.SelectInsurance(true);

        await _repo.SaveAsync(rental, ct);

        return new CreateBookingResponseDto(
            rental.RentalId,
            trailerKey,
            rental.Period.StartTime,
            rental.Period.AllowedEndTime,
            rental.InsuranceSelected,
            rental.Status.ToString()
        );
    }
}
