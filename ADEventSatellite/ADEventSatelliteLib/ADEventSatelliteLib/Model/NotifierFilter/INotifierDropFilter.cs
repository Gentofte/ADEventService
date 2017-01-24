using GK.AD;

namespace ADEventSatellite.Model
{
    // ================================================================================
    public interface INotifierDropFilter
    {
        // -----------------------------------------------------------------------------
        IADObject FilterObject(IADObject adObj);
    }
}