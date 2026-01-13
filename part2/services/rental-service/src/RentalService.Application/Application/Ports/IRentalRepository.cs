using RentalService.Domain.Aggregates;

namespace RentalService.Application.Ports;

public interface IRentalRepository
{
    Task<Rental?> GetByIdAsync(Guid rentalId, CancellationToken ct);
    Task SaveAsync(Rental rental, CancellationToken ct);

    /// <summary>
    /// Checks whether a trailer has an overlapping booking/rental for a given start time.
    /// For iteration 1, we use a simplified rule: one active/returned-but-not-closed rental blocks booking.
    /// </summary>
    Task<bool> HasActiveRentalForTrailerAsync(string trailerIdString, CancellationToken ct);
}
