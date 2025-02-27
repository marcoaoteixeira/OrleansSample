namespace Nameless.Orleans.Grains.States;

[GenerateSerializer]
public record BalanceState {
    [Id(0)]
    public decimal Amount { get; set; }
}