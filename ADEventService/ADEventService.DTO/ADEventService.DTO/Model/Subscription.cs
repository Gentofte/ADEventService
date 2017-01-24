using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ADEventService.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/adeventservice/dto/2016/v1")]
    public class Subscription : ISubscription
    {
        // -----------------------------------------------------------------------------
        [DataMember]
        [Key]
        public Guid ID { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string Name { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string Description { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string Endpoint { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string ContactEmail { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public bool Enabled { get; set; }
    }
}
