using BillingService.Domain.Events;
using BillingService.Domain.Exceptions;
using BillingService.Domain.ValueObjects;

namespace BillingService.Domain.Aggregates;

/// <summary>
/// Aggregate root for billing.
/// Invariants:
/// - Insurance fee is fixed (applied if insuranceSelected)
/// - Late fee only applied if isLate
/// - Capture can only happen once
/// </summary>
public sealed class Charge
{
    private readonly List<IDomainEvent> _events = new();
    private readonly List<LineItem> _items = new();

    public Guid ChargeId { get; private set; }
    public Guid RentalId { get; private set; }
    public ChargeStatus Status { get; private set; }

    public IReadOnlyCollection<LineItem> LineItems => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(i => i.Amount);

    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    private Charge() { }

    public static Charge CreateForRental(Guid rentalId)
    {
        var c = new Charge
        {
            ChargeId = Guid.NewGuid(),
            RentalId = rentalId,
            Status = ChargeStatus.Open
        };

        c.AddEvent(new ChargeCreated(c.ChargeId, c.RentalId, DateTime.UtcNow));
        return c;
    }

    public void AddLineItem(LineItem item)
    {
        EnsureOpen();
        _items.Add(item);
        AddEvent(new LineItemAdded(ChargeId, item.Name, item.Amount, DateTime.UtcNow));
    }

    public void MarkCaptured()
    {
        EnsureOpen();
        Status = ChargeStatus.Captured;
        AddEvent(new PaymentCaptured(ChargeId, RentalId, TotalAmount, DateTime.UtcNow));
    }

    public void MarkFailed(string reason)
    {
        EnsureOpen();
        Status = ChargeStatus.Failed;
        AddEvent(new PaymentFailed(ChargeId, RentalId, reason, DateTime.UtcNow));
    }

    public void ClearEvents() => _events.Clear();

    private void EnsureOpen()
    {
        if (Status != ChargeStatus.Open)
            throw new InvalidChargeStateException($"Charge must be Open. Current status: {Status}");
    }

    private void AddEvent(IDomainEvent ev) => _events.Add(ev);
}
