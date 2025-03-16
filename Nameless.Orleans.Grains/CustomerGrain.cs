using Nameless.Orleans.Core;
using Nameless.Orleans.Grains.Abstractions;
using Nameless.Orleans.Grains.Events;
using Nameless.Orleans.Grains.States;
using Orleans.Runtime;
using Orleans.Streams;

namespace Nameless.Orleans.Grains;

public class CustomerGrain : Grain, ICustomerGrain, IAsyncObserver<BalanceChangeEvent> {
    private readonly IPersistentState<CustomerState> _customerState;

    public CustomerGrain(
        [PersistentState(nameof(CustomerState), StorageNames.TableStorage)]
        IPersistentState<CustomerState> customerState) {
        _customerState = customerState;
    }

    public async Task AddAccountAsync(Guid accountId) {
        _customerState.State.BalanceByAccountId[accountId] = 0M;

        var streamProvider = this.GetStreamProvider(StreamNames.StreamProvider);
        var streamId = StreamId.Create(Constants.BalanceStreamNamespace, accountId);
        var stream = streamProvider.GetStream<BalanceChangeEvent>(streamId);

        await stream.SubscribeAsync(this);

        await _customerState.WriteStateAsync();
    }

    public Task<decimal> GetNetWorthAsync(Guid accountId) {
        _customerState.State
                      .BalanceByAccountId
                      .TryGetValue(accountId, out var balance);
        
        return Task.FromResult(balance);
    }

    public Task OnNextAsync(BalanceChangeEvent item, StreamSequenceToken? token = null) {
        _customerState.State.BalanceByAccountId[item.AccountId] = item.CurrentBalance;

        return _customerState.WriteStateAsync();
    }

    public Task OnCompletedAsync() => Task.CompletedTask;

    public Task OnErrorAsync(Exception ex) => Task.CompletedTask;

    public override async Task OnActivateAsync(CancellationToken cancellationToken) {
        var streamProvider = this.GetStreamProvider(StreamNames.StreamProvider);

        foreach (var accountId in _customerState.State.BalanceByAccountId.Keys) {
            var streamId = StreamId.Create(Constants.BalanceStreamNamespace, accountId);
            var stream = streamProvider.GetStream<BalanceChangeEvent>(streamId);
            var handles = await stream.GetAllSubscriptionHandles();

            foreach (var handle in handles) {
                await handle.ResumeAsync(this);
            }
        }

        await base.OnActivateAsync(cancellationToken);
    }
}