namespace Nameless.Orleans.Grains.Abstractions;

public interface ICustomerGrain : IGrainWithGuidKey {
    Task AddAccountAsync(Guid accountId);

    Task<decimal> GetNetWorthAsync(Guid accountId);
}