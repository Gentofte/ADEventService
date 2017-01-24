using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace ADEventSatellite.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/adeventsatellite/dto/2016/v1")]
    public class EventPackage
    {
        // -----------------------------------------------------------------------------
        [DataMember]
        public Guid SattelliteID { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public string ADEventAsB64 { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public string CallBackURI { get; set; }
    }
}