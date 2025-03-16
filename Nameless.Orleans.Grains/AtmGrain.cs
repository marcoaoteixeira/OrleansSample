using Microsoft.Extensions.Logging;
using Nameless.Orleans.Grains.Abstractions;
using Nameless.Orleans.Grains.States;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Transactions.Abstractions;

namespace Nameless.Orleans.Grains;

[Reentrant]
public class AtmGrain : Grain, IAtmGrain, IIncomingGrainCallFilter {
    private readonly ILogger<AtmGrain> _logger;
    private readonly ITransactionalState<AtmState> _atmState;

    public AtmGrain(
        ILogger<AtmGrain> logger,

        [TransactionalState(nameof(AtmState))]
        ITransactionalState<AtmState> atmState) {
        _logger = logger;
        _atmState = atmState;
    }

    public Task InitializeAsync(decimal openingBalance)
        => _atmState.PerformUpdate(state => {
            state.Id = this.GetGrainId().GetGuidKey();
            state.Balance = openingBalance;
        });

    public async Task WithdrawAsync(Guid accountId, decimal amount) =>
        //var accountGrain = GrainFactory.GetGrain<IAccountGrain>(accountId);
        //await accountGrain.DebitAsync(amount);
        await _atmState.PerformUpdate(state => {
            state.Balance -= amount;
        });

    public Task<decimal> GetCurrentBalanceAsync()
        => _atmState.PerformRead(state => state.Balance);

    public async Task Invoke(IIncomingGrainCallContext context) {
        _logger.LogInformation($"Incoming Grain Filter: Received grain call on '{context.Grain}' to '{context.MethodName}' method");

        await context.Invoke();
    }
}
