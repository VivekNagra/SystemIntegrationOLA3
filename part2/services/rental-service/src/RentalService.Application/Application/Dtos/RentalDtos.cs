namespace RentalService.Application.Dtos;

public sealed record CreateBookingRequestDto(
    string LocationId,
    int TrailerNumber,
    string CustomerId,
    DateTime DesiredStartTime,
    bool InsuranceSelected
);

public sealed record CreateBookingResponseDto(
    Guid RentalId,
    string TrailerId,
    DateTime StartTime,
    DateTime AllowedEndTime,
    bool InsuranceSelected,
    string Status
);

public sealed record StartRentalResponseDto(
    Guid RentalId,
    string Status
);

public sealed record ReturnRentalRequestDto(
    DateTime ReturnTime
);

public sealed record ReturnRentalResponseDto(
    Guid RentalId,
    string Status,
    bool IsLate,
    DateTime AllowedEndTime
);

public sealed record RentalDetailsResponseDto(
    Guid RentalId,
    string TrailerId,
    string CustomerId,
    DateTime StartTime,
    DateTime AllowedEndTime,
    DateTime? ReturnTime,
    bool InsuranceSelected,
    string Status
);

