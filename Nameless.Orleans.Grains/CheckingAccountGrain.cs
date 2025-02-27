using Nameless.Orleans.Grains.Abstractions;
using Nameless.Orleans.Grains.States;

namespace Nameless.Orleans.Grains;

public class CheckingAccountGrain : Grain, ICheckingAccountGrain {
    private readonly IPersistentState<BalanceState> _balanceState;
    private readonly IPersistentState<CheckingAccountState> _checkingAccountState;

    public CheckingAccountGrain(
        [PersistentState(nameof(BalanceState), "tableStorage")]
        IPersistentState<BalanceState> balanceState,
        [PersistentState(nameof(CheckingAccountState), "blobStorage")]
        IPersistentState<CheckingAccountState> checkingAccountState) {
        _balanceState = balanceState;
        _checkingAccountState = checkingAccountState;
    }

    public async Task InitializeAsync(decimal openingBalance) {
        _checkingAccountState.State.AccountId = this.GetGrainId().GetGuidKey();
        _checkingAccountState.State.OpenedAtUtc = DateTime.UtcNow;
        _checkingAccountState.State.Type = "Default";

        _balanceState.State.Amount = openingBalance;

        await _checkingAccountState.WriteStateAsync();
        await _balanceState.WriteStateAsync();
    }

    public Task<decimal> GetBalanceAsync() {
        var result = _balanceState.State.Amount;

        return Task.FromResult(result);
    }

    public Task DebitAsync(decimal value) {
        _balanceState.State.Amount -= value;

        return _balanceState.WriteStateAsync();
    }

    public Task CreditAsync(decimal value) {
        _balanceState.State.Amount += value;

        return _balanceState.WriteStateAsync();
    }
}