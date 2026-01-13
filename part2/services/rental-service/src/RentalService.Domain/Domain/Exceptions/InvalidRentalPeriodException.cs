namespace RentalService.Domain.Exceptions;

public sealed class InvalidRentalPeriodException : DomainException
{
    public InvalidRentalPeriodException(string message) : base(message) { }
}
