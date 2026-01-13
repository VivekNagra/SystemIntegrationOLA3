namespace RentalService.Domain.Exceptions;

public sealed class InvalidTrailerIdException : DomainException
{
    public InvalidTrailerIdException(string message) : base(message) { }
}
