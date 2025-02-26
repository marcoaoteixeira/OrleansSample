using Nameless.Orleans.Grains.Abstractions;

namespace Nameless.Orleans.Grains;

public class CheckingAccountGrain : Grain, ICheckingAccountGrain {
    private decimal _balance;

    public Task InitializeAsync(decimal openingBalance) {
        _balance = openingBalance;

        return Task.CompletedTask;
    }

    public Task<decimal> GetBalance() {
        return Task.FromResult(_balance);
    }
}