using Nameless.Orleans.Core;
using Nameless.Orleans.Grains.Abstractions;
using Nameless.Orleans.Grains.States;
using Orleans.Concurrency;

namespace Nameless.Orleans.Grains;

[StatelessWorker]
public class StatelessTransferGrain : Grain, IStatelessTransferGrain {
    private readonly ITransactionClient _transactionClient;
    private readonly IPersistentState<StatelessTransferState> _statelessTransferState;

    public StatelessTransferGrain(
        ITransactionClient transactionClient,

        [PersistentState(nameof(StatelessTransferState), StorageNames.TableStorage)]
        IPersistentState<StatelessTransferState> statelessTransferState) {
        _transactionClient = transactionClient;
        _statelessTransferState = statelessTransferState;
    }

    public async Task ProcessTransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount) {
        var fromAccountGrain = GrainFactory.GetGrain<IAccountGrain>(fromAccountId);
        var toAccountGrain = GrainFactory.GetGrain<IAccountGrain>(toAccountId);

        await _transactionClient.RunTransaction(TransactionOption.Create, async () => {
            await fromAccountGrain.DebitAsync(amount);
            await toAccountGrain.CreditAsync(amount);
        });

        _statelessTransferState.State.Count++;

        await _statelessTransferState.WriteStateAsync();
    }
}
