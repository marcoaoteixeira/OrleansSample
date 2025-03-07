namespace Nameless.Orleans.Grains.Abstractions;

public interface IAtmGrain : IGrainWithGuidKey {
    [Transaction(TransactionOption.Create)]
    Task InitializeAsync(decimal openingBalance);

    [Transaction(TransactionOption.CreateOrJoin)]
    Task WithdrawAsync(Guid accountId, decimal amount);

    [Transaction(TransactionOption.Create)]
    Task<decimal> GetCurrentBalanceAsync();
}
