using RentalService.Application.Dtos;
using RentalService.Application.Exceptions;
using RentalService.Application.Ports;

namespace RentalService.Application.UseCases;

public sealed class StartRentalUseCase
{
    private readonly IRentalRepository _repo;

    public StartRentalUseCase(IRentalRepository repo)
    {
        _repo = repo;
    }

    public async Task<StartRentalResponseDto> ExecuteAsync(Guid rentalId, CancellationToken ct)
    {
        var rental = await _repo.GetByIdAsync(rentalId, ct);
        if (rental is null)
            throw new NotFoundException($"Rental '{rentalId}' was not found.");

        rental.Start();
        await _repo.SaveAsync(rental, ct);

        return new StartRentalResponseDto(rental.RentalId, rental.Status.ToString());
    }
}

