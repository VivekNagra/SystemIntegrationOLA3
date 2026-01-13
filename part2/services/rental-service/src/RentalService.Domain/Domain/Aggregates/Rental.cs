using RentalService.Domain.Events;
using RentalService.Domain.Exceptions;
using RentalService.Domain.ValueObjects;

namespace RentalService.Domain.Aggregates;

/// <summary>
/// Aggregate root for the short-term rental lifecycle.
/// Invariants:
/// - allowedEndTime = min(start + 24h, midnight cut-off)
/// - start only from Booked
/// - return only from Active
/// - close only after Returned (and billing completed externally)
/// - if returnTime > allowedEndTime => LateReturnDetected must be emitted
/// </summary>
public sealed class Rental
{
    private readonly List<IDomainEvent> _events = new();

    public Guid RentalId { get; private set; }
    public TrailerId TrailerId { get; private set; }
    public string CustomerId { get; private set; } = string.Empty;

    public RentalPeriod Period { get; private set; } = null!;
    public DateTime? ReturnTime { get; private set; }

    public RentalStatus Status { get; private set; }
    public bool InsuranceSelected { get; private set; }

    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    private Rental() { }

    public static Rental Book(TrailerId trailerId, string customerId, DateTime desiredStartTime)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new DomainExceptionImpl("customerId must be provided.");

        var rental = new Rental
        {
            RentalId = Guid.NewGuid(),
            TrailerId = trailerId,
            CustomerId = customerId.Trim(),
            Period = RentalPeriod.Create(desiredStartTime),
            Status = RentalStatus.Booked,
            InsuranceSelected = false
        };

        rental.AddEvent(new BookingAccepted(
            rental.RentalId,
            rental.TrailerId,
            rental.CustomerId,
            rental.Period.StartTime,
            rental.Period.AllowedEndTime,
            DateTime.UtcNow
        ));

        return rental;
    }

    public void SelectInsurance(bool selected)
    {
        EnsureNotClosed();

        InsuranceSelected = selected;
        AddEvent(new InsuranceSelected(RentalId, selected, DateTime.UtcNow));
    }

    public void Start()
    {
        if (Status != RentalStatus.Booked)
            throw new InvalidRentalStateException($"Rental must be Booked to start. Current status: {Status}");

        Status = RentalStatus.Active;
        AddEvent(new RentalStarted(RentalId, Period.StartTime, DateTime.UtcNow));
    }

    public void Return(DateTime returnTime)
    {
        if (Status != RentalStatus.Active)
            throw new InvalidRentalStateException($"Rental must be Active to return. Current status: {Status}");

        ReturnTime = returnTime;
        Status = RentalStatus.Returned;

        AddEvent(new TrailerReturned(RentalId, returnTime, DateTime.UtcNow));

        if (Period.IsLate(returnTime))
        {
            AddEvent(new LateReturnDetected(RentalId, returnTime, Period.AllowedEndTime, DateTime.UtcNow));
        }
    }

    public void Close()
    {
        if (Status != RentalStatus.Returned)
            throw new InvalidRentalStateException($"Rental must be Returned to close. Current status: {Status}");

        Status = RentalStatus.Closed;
        AddEvent(new RentalClosed(RentalId, DateTime.UtcNow));
    }

    public void ClearEvents() => _events.Clear();

    private void EnsureNotClosed()
    {
        if (Status == RentalStatus.Closed)
            throw new InvalidRentalStateException("Rental is already Closed.");
    }

    private void AddEvent(IDomainEvent ev) => _events.Add(ev);

    // Small internal exception type to avoid over-creating files for one message.
    private sealed class DomainExceptionImpl : DomainException
    {
        public DomainExceptionImpl(string message) : base(message) { }
    }
}
