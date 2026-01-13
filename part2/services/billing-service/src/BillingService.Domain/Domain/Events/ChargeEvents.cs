namespace BillingService.Domain.Events;

public sealed record ChargeCreated(Guid ChargeId, Guid RentalId, DateTime OccurredAt) : IDomainEvent;

public sealed record LineItemAdded(Guid ChargeId, string Name, decimal Amount, DateTime OccurredAt) : IDomainEvent;

public sealed record PaymentCaptured(Guid ChargeId, Guid RentalId, decimal TotalAmount, DateTime OccurredAt) : IDomainEvent;

public sealed record PaymentFailed(Guid ChargeId, Guid RentalId, string Reason, DateTime OccurredAt) : IDomainEvent;
