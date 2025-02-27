using System.Runtime.Serialization;

namespace Nameless.Orleans.Client.Contracts;

[DataContract]
public record Debit {
    [DataMember]
    public decimal Amount { get; init; }
}