using RentalService.Domain.ValueObjects;

namespace RentalService.Domain.Events;

public sealed record BookingAccepted(
    Guid RentalId,
    TrailerId TrailerId,
    string CustomerId,
    DateTime StartTime,
    DateTime AllowedEndTime,
    DateTime OccurredAt
) : IDomainEvent;

public sealed record InsuranceSelected(
    Guid RentalId,
    bool Selected,
    DateTime OccurredAt
) : IDomainEvent;

public sealed record RentalStarted(
    Guid RentalId,
    DateTime StartTime,
    DateTime OccurredAt
) : IDomainEvent;

public sealed record TrailerReturned(
    Guid RentalId,
    DateTime ReturnTime,
    DateTime OccurredAt
) : IDomainEvent;

public sealed record LateReturnDetected(
    Guid RentalId,
    DateTime ReturnTime,
    DateTime AllowedEndTime,
    DateTime OccurredAt
) : IDomainEvent;

public sealed record RentalClosed(
    Guid RentalId,
    DateTime OccurredAt
) : IDomainEvent;
