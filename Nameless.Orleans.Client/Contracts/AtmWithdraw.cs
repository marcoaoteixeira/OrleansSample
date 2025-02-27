using System.Runtime.Serialization;

namespace Nameless.Orleans.Client.Contracts;

[DataContract]
public record AtmWithdraw {
    [DataMember]
    public Guid AccountId { get; init; }

    [DataMember]
    public decimal Amount { get; init; }
}