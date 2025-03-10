namespace Nameless.Orleans.Grains.Abstractions;

public interface IStatelessTransferGrain : IGrainWithIntegerKey {
    Task ProcessTransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount);
}
