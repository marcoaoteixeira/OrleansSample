namespace Nameless.Orleans.Grains.States;

[GenerateSerializer]
public record CustomerState {
    [Id(0)]
    public Dictionary<Guid, decimal> BalanceByAccountId { get; set; } = [];
}
