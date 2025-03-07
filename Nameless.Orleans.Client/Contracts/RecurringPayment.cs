namespace Nameless.Orleans.Client.Contracts;

public record RecurringPayment {
    public decimal Amount { get; init; }
    public int PeriodInMinutes { get; set; }
}
