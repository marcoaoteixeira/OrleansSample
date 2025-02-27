namespace Nameless.Orleans.Grains.Abstractions;

public interface ICheckingAccountGrain : IGrainWithGuidKey {
    Task InitializeAsync(decimal openingBalance);
    Task<decimal> GetBalanceAsync();
    Task DebitAsync(decimal value);
    Task CreditAsync(decimal value);
}