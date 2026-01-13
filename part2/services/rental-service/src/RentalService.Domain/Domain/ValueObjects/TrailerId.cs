using RentalService.Domain.Exceptions;

namespace RentalService.Domain.ValueObjects;

public readonly record struct TrailerId
{
    public string LocationId { get; }
    public int TrailerNumber { get; }

    public TrailerId(string locationId, int trailerNumber)
    {
        if (string.IsNullOrWhiteSpace(locationId))
            throw new InvalidTrailerIdException("locationId must be provided.");

        if (trailerNumber <= 0)
            throw new InvalidTrailerIdException("trailerNumber must be a positive integer.");

        LocationId = locationId.Trim();
        TrailerNumber = trailerNumber;
    }

    public override string ToString() => $"{LocationId}:{TrailerNumber}";
}
