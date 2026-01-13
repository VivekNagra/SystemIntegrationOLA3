using BillingService.Domain.Aggregates;

namespace BillingService.Application.Ports;

public interface IChargeRepository
{
    Task<Charge?> GetByIdAsync(Guid chargeId, CancellationToken ct);
    Task SaveAsync(Charge charge, CancellationToken ct);
}
