
namespace GK.AD.DTO
{
    // ================================================================================
    public interface IOU : IADObject
    {
        string street { get; set; }
        string l_aka_city { get; set; }
        string st_aka_stateprovince { get; set; }
        string postalCode { get; set; }
        string managedBy { get; set; }

        string extID { get; set; }
        string shortKey { get; set; }
    }
}
