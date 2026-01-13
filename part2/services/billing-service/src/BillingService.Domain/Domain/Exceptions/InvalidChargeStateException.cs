namespace BillingService.Domain.Exceptions;

public sealed class InvalidChargeStateException : DomainException
{
    public InvalidChargeStateException(string message) : base(message) { }
}
