namespace RentalService.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
