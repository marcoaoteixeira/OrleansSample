using System.Runtime.Serialization;

namespace Nameless.Orleans.Client.Contracts;

[DataContract]
public record CreateAtm {
    [DataMember]
    public decimal OpeningBalance { get; init; }
}