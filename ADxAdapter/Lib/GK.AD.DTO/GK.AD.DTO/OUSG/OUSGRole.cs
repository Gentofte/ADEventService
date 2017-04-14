using System;

using System.Runtime.Serialization;

namespace GK.AD.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    public class OUSGRole
    {
        // -----------------------------------------------------------------------------
        [DataMember]
        public string Role { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public Group Group { get; set; }
    }
}
