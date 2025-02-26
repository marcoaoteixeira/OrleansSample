using System.Runtime.Serialization;

namespace Nameless.Orleans.Client.Contracts {
    [DataContract]
    public record CreateAccount {
        [DataMember]
        public decimal OpeningBalance { get; init; }
    }
}
