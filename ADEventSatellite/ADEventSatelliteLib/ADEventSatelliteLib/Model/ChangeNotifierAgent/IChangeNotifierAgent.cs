using GK.AD;
using System;

namespace ADEventSatellite.Model
{
    // ================================================================================
    public interface IChangeNotifierAgent
    {
        // -----------------------------------------------------------------------------
        ChangeNotifier NewChangeNotifier(EventHandler<ADObjectChangedEventArgs> handler);

        // -----------------------------------------------------------------------------
        void DisposeChangeNotifier(ref ChangeNotifier cn);
    }
}