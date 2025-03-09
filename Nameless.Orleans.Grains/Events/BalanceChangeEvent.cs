namespace Nameless.Orleans.Grains.Events;

[GenerateSerializer]
public record BalanceChangeEvent {
    [Id(0)]
    public Guid AccountId { get; init; }

    [Id(1)]
    public decimal CurrentBalance { get; init; }
}
