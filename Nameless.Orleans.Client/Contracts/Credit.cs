using System.Runtime.Serialization;

namespace Nameless.Orleans.Client.Contracts;

[DataContract]
public record Credit {
    [DataMember]
    public decimal Amount { get; init; }
}