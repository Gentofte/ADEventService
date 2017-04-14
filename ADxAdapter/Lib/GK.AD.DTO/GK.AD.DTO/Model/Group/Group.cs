using System;
using System.Runtime.Serialization;

namespace GK.AD.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    public enum GroupType
    {
        [EnumMember]
        Security = 0,
        [EnumMember]
        Distribution,
        [EnumMember]
        Other
    }

    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    public enum GroupScope
    {
        [EnumMember]
        Local = 0,
        [EnumMember]
        Global,
        [EnumMember]
        Universal,
        [EnumMember]
        System
    }

    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    public class Group : IGroup
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
        public string sAMAccountName { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string mail { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string info { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string managedBy { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string objectSID { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public GroupType groupType { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public GroupScope groupScope { get; set; }
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
