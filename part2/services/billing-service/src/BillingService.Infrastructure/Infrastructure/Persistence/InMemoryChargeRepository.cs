using System.Collections.Concurrent;
using BillingService.Application.Ports;
using BillingService.Domain.Aggregates;

namespace BillingService.Infrastructure.Persistence;

/// <summary>
/// Simple in-memory repository for iteration 1.
/// </summary>
public sealed class InMemoryChargeRepository : IChargeRepository
{
    private readonly ConcurrentDictionary<Guid, Charge> _store = new();

    public Task<Charge?> GetByIdAsync(Guid chargeId, CancellationToken ct)
    {
        _store.TryGetValue(chargeId, out var charge);
        return Task.FromResult(charge);
    }

    public Task SaveAsync(Charge charge, CancellationToken ct)
    {
        _store[charge.ChargeId] = charge;
        return Task.CompletedTask;
    }
}
