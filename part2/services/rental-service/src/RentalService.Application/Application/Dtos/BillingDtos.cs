namespace RentalService.Application.Dtos;

public sealed record CreateChargeRequestDto(
    Guid RentalId,
    bool InsuranceSelected,
    bool IsLate
);

public sealed record CreateChargeResponseDto(
    Guid ChargeId,
    decimal TotalAmount,
    string PaymentStatus
);
