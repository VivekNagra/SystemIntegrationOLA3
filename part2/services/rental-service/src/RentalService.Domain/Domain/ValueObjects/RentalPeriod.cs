using RentalService.Domain.Exceptions;

namespace RentalService.Domain.ValueObjects;

public sealed record RentalPeriod
{
    public DateTime StartTime { get; }
    public DateTime AllowedEndTime { get; }

    private RentalPeriod(DateTime startTime, DateTime allowedEndTime)
    {
        StartTime = startTime;
        AllowedEndTime = allowedEndTime;
    }

    public static RentalPeriod Create(DateTime startTime)
    {
        // Compute midnight cut-off as 23:59:59 on the start day (same day).
        var midnightCutoff = new DateTime(
            startTime.Year, startTime.Month, startTime.Day,
            23, 59, 59,
            startTime.Kind
        );

        var max24h = startTime.AddHours(24);

        var allowedEnd = max24h <= midnightCutoff ? max24h : midnightCutoff;

        if (allowedEnd < startTime)
            throw new InvalidRentalPeriodException("AllowedEndTime cannot be earlier than StartTime.");

        return new RentalPeriod(startTime, allowedEnd);
    }

    public bool IsLate(DateTime returnTime) => returnTime > AllowedEndTime;
}
