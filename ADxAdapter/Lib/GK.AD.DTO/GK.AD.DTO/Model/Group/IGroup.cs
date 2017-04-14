
namespace GK.AD.DTO
{
    // ================================================================================
    public interface IGroup : IADObject
    {
        string sAMAccountName { get; set; }
        string mail { get; set; }
        string info { get; set; }
        string managedBy { get; set; }
        string objectSID { get; set; }
        GroupType groupType { get; set; }
        GroupScope groupScope { get; set; }

        string extID { get; set; }
        string shortKey { get; set; }
    }
}
