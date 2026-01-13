using BillingService.Application.Dtos;
using BillingService.Application.Ports;
using BillingService.Domain.Aggregates;
using BillingService.Domain.ValueObjects;

namespace BillingService.Application.UseCases;

/// <summary>
/// Use case for creating a charge based on rental outcome.
/// Iteration 1 fee rules:
/// - Insurance fee is fixed at 50
/// - Late fee is fixed at 100 (demo assumption)
/// - Payment is captured via IPaymentProvider (fake in iteration 1)
/// </summary>
public sealed class CreateChargeFromRentalUseCase
{
    public const decimal InsuranceFee = 50m;
    public const decimal LateFee = 100m; // iteration 1 assumption for demo

    private readonly IChargeRepository _repo;
    private readonly IPaymentProvider _paymentProvider;

    public CreateChargeFromRentalUseCase(IChargeRepository repo, IPaymentProvider paymentProvider)
    {
        _repo = repo;
        _paymentProvider = paymentProvider;
    }

    public async Task<CreateChargeFromRentalResponseDto> ExecuteAsync(CreateChargeFromRentalRequestDto req, CancellationToken ct)
    {
        var charge = Charge.CreateForRental(req.RentalId);

        if (req.InsuranceSelected)
            charge.AddLineItem(LineItem.InsuranceFee(InsuranceFee));

        if (req.IsLate)
            charge.AddLineItem(LineItem.LateFee(LateFee));

        // In this case study the base rental fee is 0, so we only capture if any add-ons exist.
        // Capturing 0 is allowed but unnecessary; we treat it as captured for simplicity.
        var paymentCaptured = charge.TotalAmount <= 0m || await _paymentProvider.CaptureAsync(charge, ct);

        if (paymentCaptured)
        {
            charge.MarkCaptured();
        }
        else
        {
            charge.MarkFailed("Payment capture failed (iteration 1 fake provider).");
        }

        await _repo.SaveAsync(charge, ct);

        return new CreateChargeFromRentalResponseDto(
            charge.ChargeId,
            charge.TotalAmount,
            charge.Status == ChargeStatus.Captured ? "Captured" : "Failed"
        );
    }
}
