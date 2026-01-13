using BillingService.Application.Ports;
using BillingService.Domain.Aggregates;

namespace BillingService.Infrastructure.Payments;


public sealed class FakePaymentProvider : IPaymentProvider
{
    public Task<bool> CaptureAsync(Charge charge, CancellationToken ct)
    {
      
        return Task.FromResult(true);
    }
}
