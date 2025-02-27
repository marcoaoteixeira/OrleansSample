namespace Nameless.Orleans.Grains.Abstractions;

public interface IAtmGrain : IGrainWithGuidKey {
    Task InitializeAsync(decimal openingBalance);
    Task WithdrawAsync(Guid accountId, decimal amount);
}
