namespace RentalService.Domain.Exceptions;

public sealed class InvalidRentalStateException : DomainException
{
    public InvalidRentalStateException(string message) : base(message) { }
}
