using System;

using System.Runtime.Serialization;

namespace GK.AD.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    public class WhoWhatWhen : IWhoWhatWhen
    {
        // -----------------------------------------------------------------------------
        [DataMember]
        public string Who { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public string What { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public DateTime When { get; set; }
    }
}
