using RentalService.Application.Dtos;
using RentalService.Application.Exceptions;
using RentalService.Application.Ports;

namespace RentalService.Application.UseCases;

public sealed class GetRentalUseCase
{
    private readonly IRentalRepository _repo;

    public GetRentalUseCase(IRentalRepository repo)
    {
        _repo = repo;
    }

    public async Task<RentalDetailsResponseDto> ExecuteAsync(Guid rentalId, CancellationToken ct)
    {
        var rental = await _repo.GetByIdAsync(rentalId, ct);
        if (rental is null)
            throw new NotFoundException($"Rental '{rentalId}' was not found.");

        return new RentalDetailsResponseDto(
            rental.RentalId,
            rental.TrailerId.ToString(),
            rental.CustomerId,
            rental.Period.StartTime,
            rental.Period.AllowedEndTime,
            rental.ReturnTime,
            rental.InsuranceSelected,
            rental.Status.ToString()
        );
    }
}
