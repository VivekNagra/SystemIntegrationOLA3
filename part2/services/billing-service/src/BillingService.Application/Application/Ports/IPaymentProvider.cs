using BillingService.Domain.Aggregates;

namespace BillingService.Application.Ports;

/// <summary>
/// Port for capturing payment in an external system.
/// In iteration 1, this will be implemented as a fake adapter.
/// </summary>
public interface IPaymentProvider
{
    Task<bool> CaptureAsync(Charge charge, CancellationToken ct);
}
