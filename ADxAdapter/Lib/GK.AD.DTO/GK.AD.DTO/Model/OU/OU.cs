using System;
using System.Runtime.Serialization;

namespace GK.AD.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    public class OU : IOU
    {
        #region Common AD object attributes
        // -----------------------------------------------------------------------------
        [DataMember]
        public ObjectClass objectClass { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public Guid objectGuid { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string dn { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string canonicalName { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string name { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string description { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string displayName { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public bool isDeleted { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public int objectVersion { get; set; }
        #endregion

        // -----------------------------------------------------------------------------
        [DataMember]
        public string street { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string l_aka_city { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string st_aka_stateprovince { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string postalCode { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string managedBy { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string extID { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string shortKey { get; set; }

        // -----------------------------------------------------------------------------
        public string ToString(bool compact = true)
        {
            return Formatter.ToString(this, compact);
        }
    }
}
