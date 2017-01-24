using System;
using System.Runtime.Serialization;

namespace ADEventService.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/adeventservice/dto/2016/v1")]
    public class CreateSubscriptionRequest : ICreateSubscriptionRequest
    {
        // -----------------------------------------------------------------------------
        [DataMember]
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
    }
}
