using GK.AD;

namespace ADEventSatellite.Model
{
    // ================================================================================
    public class NoFilter : INotifierDropFilter
    {
        // -----------------------------------------------------------------------------
        public IADObject FilterObject(IADObject adObj)
        {
            return adObj;
        }
    }
}