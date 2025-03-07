namespace Nameless.Orleans.Grains.Abstractions;

public interface IAccountGrain : IGrainWithGuidKey {
    [Transaction(TransactionOption.Create)]
    Task InitializeAsync(decimal openingBalance);

    [Transaction(TransactionOption.Create)]
    Task<decimal> GetCurrentBalanceAsync();

    // Withdraw method in AtmGrain could have
    // created a transaction, so that's why
    // we are specifying CreateOrJoin
    [Transaction(TransactionOption.CreateOrJoin)]
    Task DebitAsync(decimal value);

    [Transaction(TransactionOption.CreateOrJoin)]
    Task CreditAsync(decimal value);
    
    Task AddRecurringPaymentAsync(decimal amount, int periodInMinutes);
}