using System.Runtime.Serialization;

namespace Nameless.Orleans.Client.Contracts;

[DataContract]
public record CustomerAccount {
    [DataMember]
    public Guid AccountId { get; init; }
}
