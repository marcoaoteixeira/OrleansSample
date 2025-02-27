namespace Nameless.Orleans.Grains.States;

[GenerateSerializer]
public record AtmState {
    [Id(0)]
    public Guid Id { get; set; }

    [Id(1)]
    public decimal Balance { get; set; }
}
