using System;

using System.Runtime.Serialization;

namespace GK.AD.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    public enum ObjectClass
    {
        [EnumMember]
        undefined,
        [EnumMember]
        user,
        [EnumMember]
        group,
        [EnumMember]
        organizationalUnit,
        [EnumMember]
        computer,
        [EnumMember]
        printQueue,
        [EnumMember]
        otherClass,
        [EnumMember]
        container
    }
}
