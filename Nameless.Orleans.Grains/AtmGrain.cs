using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nameless.Orleans.Grains.Abstractions;
using Nameless.Orleans.Grains.States;

namespace Nameless.Orleans.Grains;

public class AtmGrain : Grain, IAtmGrain {
    private readonly IPersistentState<AtmState> _atmState;

    public AtmGrain(
        [PersistentState(nameof(AtmState), "tableStorage")]
        IPersistentState<AtmState> atmState) {
        _atmState = atmState;
    }

    public async Task InitializeAsync(decimal openingBalance) {
        _atmState.State.Id = this.GetGrainId().GetGuidKey();
        _atmState.State.Balance = openingBalance;

        await _atmState.WriteStateAsync();
    }

    public async Task WithdrawAsync(Guid accountId, decimal amount) {
        var accountGrain = GrainFactory.GetGrain<ICheckingAccountGrain>(accountId);

        await accountGrain.DebitAsync(amount);

        _atmState.State.Balance -= amount;

        await _atmState.WriteStateAsync();
    }
}
