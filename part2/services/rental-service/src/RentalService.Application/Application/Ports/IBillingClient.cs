using RentalService.Application.Dtos;

namespace RentalService.Application.Ports;

public interface IBillingClient
{
    Task<CreateChargeResponseDto> CreateAndCaptureChargeAsync(CreateChargeRequestDto request, CancellationToken ct);
}
