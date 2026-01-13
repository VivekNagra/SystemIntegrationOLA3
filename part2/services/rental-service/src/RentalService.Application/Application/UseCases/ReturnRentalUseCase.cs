using RentalService.Application.Dtos;
using RentalService.Application.Exceptions;
using RentalService.Application.Ports;

namespace RentalService.Application.UseCases;

public sealed class ReturnRentalUseCase
{
    private readonly IRentalRepository _repo;
    private readonly IBillingClient _billing;

    public ReturnRentalUseCase(IRentalRepository repo, IBillingClient billing)
    {
        _repo = repo;
        _billing = billing;
    }

    public async Task<ReturnRentalResponseDto> ExecuteAsync(Guid rentalId, ReturnRentalRequestDto req, CancellationToken ct)
    {
        var rental = await _repo.GetByIdAsync(rentalId, ct);
        if (rental is null)
            throw new NotFoundException($"Rental '{rentalId}' was not found.");

        rental.Return(req.ReturnTime);

        var isLate = rental.Period.IsLate(req.ReturnTime);

        // Call Billing service to create and capture charge (iteration 1)
        var billingReq = new CreateChargeRequestDto(rental.RentalId, rental.InsuranceSelected, isLate);
        var billingRes = await _billing.CreateAndCaptureChargeAsync(billingReq, ct);

        // Close rental only if payment captured
        if (string.Equals(billingRes.PaymentStatus, "Captured", StringComparison.OrdinalIgnoreCase))
        {
            rental.Close();
        }

        await _repo.SaveAsync(rental, ct);

        return new ReturnRentalResponseDto(
            rental.RentalId,
            rental.Status.ToString(),
            isLate,
            rental.Period.AllowedEndTime
        );
    }
}
