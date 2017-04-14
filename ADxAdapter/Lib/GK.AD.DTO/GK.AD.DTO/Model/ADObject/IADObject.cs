using System;

namespace GK.AD.DTO
{
    // ================================================================================
    public interface IADObject
    {
        ObjectClass objectClass { get; set; }
        Guid objectGuid { get; set; }
        string dn { get; set; }
        string canonicalName { get; set; }
        string name { get; set; }
        string description { get; set; }
        string displayName { get; set; }
        bool isDeleted { get; set; }
        int objectVersion { get; set; }

        // -----------------------------------------------------------------------------
        string ToString(bool compact = true);
    }
}
