namespace Nameless.Orleans.Grains.States;

[GenerateSerializer]
public record RecurringPayment {
    [Id(0)]
    public Guid Id { get; set; }

    [Id(1)]
    public decimal Amount { get; set; }

    [Id(2)]
    public TimeSpan Period { get; set; }
}
