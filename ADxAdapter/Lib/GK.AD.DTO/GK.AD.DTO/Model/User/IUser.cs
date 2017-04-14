using System;

namespace GK.AD.DTO
{
    // ================================================================================
    public interface IUser : IADObject
    {
        string sAMAccountName { get; set; }
        string userPrincipalName { get; set; }
        string givenName { get; set; }
        string initials { get; set; }
        string sn { get; set; }
        string title { get; set; }
        string manager { get; set; }
        string department { get; set; }
        string streetAddress { get; set; }
        string physicalDeliveryOfficeName { get; set; }
        string mail { get; set; }
        string telephoneNumber { get; set; }
        string mobile { get; set; }
        string objectSID { get; set; }
        bool AccountLockedOut { get; set; }
        bool AccountEnabled { get; set; }
        bool PasswordExpired { get; set; }
        bool DontExpirePasswordEnabled { get; set; }
        DateTime AccountExpiresDT { get; set; }

        string extID { get; set; }
        string shortKey { get; set; }
    }
}
