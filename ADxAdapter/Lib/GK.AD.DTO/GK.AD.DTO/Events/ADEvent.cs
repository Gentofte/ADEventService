using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace GK.AD.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    public enum ADEventType
    {
        [EnumMember]
        Nothing = -1,
        [EnumMember]
        Create,
        [EnumMember]
        Update,
        [EnumMember]
        Delete,
        [EnumMember]
        Ping,
        [EnumMember]
        Raw
    }

    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/ad/dto/2013/v1")]
    [KnownType(typeof(User))]
    [KnownType(typeof(OU))]
    [KnownType(typeof(Group))]
    public class ADEvent : IADEvent
    {
        // -----------------------------------------------------------------------------
        [DataMember]
        public string CorrID { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public WhoWhatWhen Sender { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public ADEventType ADEventType { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public IADObject ADObject { get; set; }

        // -----------------------------------------------------------------------------
        public string ToString(bool compact = true)
        {
            var eType = (ADEventType == ADEventType.Delete) ?
                ADEventType.ToString().ToUpperInvariant() :
                ADEventType.ToString();

            if (compact)
                return string.Format(
                    "DTO:ADE=[{0}/{1}/{2}/{3}]",
                    eType,
                    CorrID.ToUpperInvariant(),
                    Sender.When.ToString(@"yyyyMMdd\:HHmmss"),
                    Formatter.ToString(ADObject, compact)
                    );
            else
                return string.Format(
                    "DTO:ADEVNT:type/ID/time/obj=[{0}/{1}/{2}/{3}]",
                    eType,
                    CorrID.ToUpperInvariant(),
                    Sender.When.ToString(@"yyyyMMdd\:HHmmss"),
                    Formatter.ToString(ADObject, compact)
                    );
        }
    }
}
