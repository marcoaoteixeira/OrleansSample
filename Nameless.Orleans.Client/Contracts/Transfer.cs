using System.Runtime.Serialization;

namespace Nameless.Orleans.Client.Contracts;

[DataContract]
public record Transfer {
    [DataMember]
    public Guid FromAccountId { get; init; }
    
    [DataMember]
    public Guid ToAccountId { get; init; }

    [DataMember]
    public decimal Amount { get; init; }
}
