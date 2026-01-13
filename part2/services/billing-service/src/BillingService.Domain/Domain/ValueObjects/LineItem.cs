namespace BillingService.Domain.ValueObjects;

public sealed record LineItem(string Name, decimal Amount)
{
    public static LineItem InsuranceFee(decimal amount) => new("Insurance", amount);
    public static LineItem LateFee(decimal amount) => new("LateFee", amount);
}
