using System;

using System.Runtime.Serialization;

namespace GK.AD.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    public class User : IUser
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
        public string userPrincipalName { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string givenName { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string initials { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string sn { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string title { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string manager { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string department { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string streetAddress { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string physicalDeliveryOfficeName { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string mail { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string telephoneNumber { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string mobile { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public string objectSID { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public bool AccountLockedOut { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public bool AccountEnabled { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public bool PasswordExpired { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public bool DontExpirePasswordEnabled { get; set; }
        // -----------------------------------------------------------------------------
        [DataMember]
        public DateTime AccountExpiresDT { get; set; }
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
