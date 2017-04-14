
namespace GK.AD.DTO
{
    // ================================================================================
    public interface IADEvent
    {
        string CorrID { get; set; }
        WhoWhatWhen Sender { get; }
        ADEventType ADEventType { get; set; }
        IADObject ADObject { get; set; }

        // -----------------------------------------------------------------------------
        string ToString(bool compact = true);
    }
}
