namespace BillingService.Application.Dtos;

public sealed record CreateChargeFromRentalRequestDto(
    Guid RentalId,
    bool InsuranceSelected,
    bool IsLate
);

public sealed record CreateChargeFromRentalResponseDto(
    Guid ChargeId,
    decimal TotalAmount,
    string PaymentStatus
);
