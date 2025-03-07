namespace Nameless.Orleans.Grains.States;

[GenerateSerializer]
public record AccountState {
    [Id(0)]
    public Guid AccountId { get; set; }

    [Id(1)]
    public DateTime OpenedAtUtc { get; set; }

    [Id(2)]
    public string Type { get; set; } = string.Empty;

    [Id(3)]
    public IList<RecurringPayment> RecurringPayments { get; init; } = [];
}