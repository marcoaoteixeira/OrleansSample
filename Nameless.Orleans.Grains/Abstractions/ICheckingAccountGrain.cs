namespace Nameless.Orleans.Grains.Abstractions;

public interface ICheckingAccountGrain : IGrainWithGuidKey {
    Task InitializeAsync(decimal openingBalance);
    Task<decimal> GetBalance();
}