using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace GK.AD.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    public class ADObject : IADObject
    {
        [DataMember]
        public ObjectClass objectClass { get; set; }
        [DataMember]
        public Guid objectGuid { get; set; }
        [DataMember]
        public string dn { get; set; }
        [DataMember]
        public string canonicalName { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string displayName { get; set; }
        [DataMember]
        public bool isDeleted { get; set; }
        [DataMember]
        public int objectVersion { get; set; }

        // -----------------------------------------------------------------------------
        public string ToString(bool compact = true)
        {
            return Formatter.ToString(this, compact);
        }
    }
}
