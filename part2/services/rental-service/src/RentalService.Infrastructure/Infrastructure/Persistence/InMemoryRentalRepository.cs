using System.Collections.Concurrent;
using RentalService.Application.Ports;
using RentalService.Domain.Aggregates;

namespace RentalService.Infrastructure.Persistence;

/// <summary>
/// Simple in-memory repository for iteration 1.
/// Not production-safe; sufficient for demoing DDD structure and service integration.
/// </summary>
public sealed class InMemoryRentalRepository : IRentalRepository
{
    private readonly ConcurrentDictionary<Guid, Rental> _store = new();

    public Task<Rental?> GetByIdAsync(Guid rentalId, CancellationToken ct)
    {
        _store.TryGetValue(rentalId, out var rental);
        return Task.FromResult(rental);
    }

    public Task SaveAsync(Rental rental, CancellationToken ct)
    {
        _store[rental.RentalId] = rental;
        return Task.CompletedTask;
    }

    public Task<bool> HasActiveRentalForTrailerAsync(string trailerIdString, CancellationToken ct)
    {
        // Simplified availability check:
        // If there exists a rental for this trailer that is not Closed, then block booking.
        var has = _store.Values.Any(r =>
            r.TrailerId.ToString().Equals(trailerIdString, StringComparison.OrdinalIgnoreCase) &&
            r.Status != RentalStatus.Closed
        );

        return Task.FromResult(has);
    }
}
