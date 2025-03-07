using Nameless.Orleans.Grains.Abstractions;
using Nameless.Orleans.Grains.States;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;

namespace Nameless.Orleans.Grains;

[Reentrant]
public class AccountGrain : Grain, IAccountGrain, IRemindable {
    private readonly ITransactionClient _transactionClient;
    private readonly ITransactionalState<BalanceState> _balanceState;
    private readonly IPersistentState<AccountState> _accountState;

    public AccountGrain(
        ITransactionClient transactionClient,

        [TransactionalState(nameof(BalanceState))]
        ITransactionalState<BalanceState> balanceState,

        [PersistentState(nameof(AccountState), "blobStorage")]
        IPersistentState<AccountState> accountState) {
        _transactionClient = transactionClient;
        _balanceState = balanceState;
        _accountState = accountState;
    }

    public async Task InitializeAsync(decimal openingBalance) {
        _accountState.State.AccountId = this.GetGrainId().GetGuidKey();
        _accountState.State.OpenedAtUtc = DateTime.UtcNow;
        _accountState.State.Type = "Default";

        await _balanceState.PerformUpdate(state => {
            state.Amount = openingBalance;
        });

        await _accountState.WriteStateAsync();
    }

    // Although the BalanceState does not have an Id
    // field, Orleans makes sure that this object belongs
    // to the current identity (AccountGrain)
    public Task<decimal> GetCurrentBalanceAsync()
        => _balanceState.PerformRead(state => state.Amount);

    public Task DebitAsync(decimal value)
        => _balanceState.PerformUpdate(state => {
            state.Amount -= value;
        });

    public Task CreditAsync(decimal value)
        => _balanceState.PerformUpdate(state => {
            state.Amount += value;
        });

    public async Task AddRecurringPaymentAsync(decimal amount, int periodInMinutes) {
        var recurringPayment = new RecurringPayment {
            Id = Guid.NewGuid(),
            Amount = amount,
            Period = TimeSpan.FromMinutes(periodInMinutes)
        };

        _accountState.State.RecurringPayments.Add(recurringPayment);

        await _accountState.WriteStateAsync();

        await this.RegisterOrUpdateReminder(reminderName: $"{nameof(RecurringPayment)}::{recurringPayment.Id}",
                                            dueTime: recurringPayment.Period,
                                            period: recurringPayment.Period);
    }

    public Task ReceiveReminder(string reminderName, TickStatus status) {
        // implement any strategy that makes sense to identify the reminder
        // origin.
        if (!reminderName.StartsWith(nameof(RecurringPayment))) {
            return Task.CompletedTask;
        }

        var recurringPaymentId = Guid.Parse(reminderName.Split("::").Last());
        var recurringPayment = _accountState.State
                                            .RecurringPayments
                                            .Single(item => item.Id == recurringPaymentId);

        return _transactionClient.RunTransaction(transactionOption: TransactionOption.Create,
                                                 transactionDelegate: () => DebitAsync(recurringPayment.Amount));
    }
}