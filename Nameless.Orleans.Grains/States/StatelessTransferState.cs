namespace Nameless.Orleans.Grains.States;

[GenerateSerializer]
public record StatelessTransferState {
    [Id(0)]
    public int Count { get; set; }
}
