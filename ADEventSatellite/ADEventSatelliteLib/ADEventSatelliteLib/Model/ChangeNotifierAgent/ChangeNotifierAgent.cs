using GK.AD;
using GK.AD.Configuration;
using GK.AppCore.Utility;
using System;

namespace ADEventSatellite.Model
{
    // ================================================================================
    public class ChangeNotifierAgent : IChangeNotifierAgent
    {
        readonly IADConfig _adConfig;

        // -----------------------------------------------------------------------------
        public ChangeNotifierAgent(IADConfig adConfig)
        {
            _adConfig = Verify.ArgumentNotNull(adConfig, "adConfig");
        }

        // -----------------------------------------------------------------------------
        public ChangeNotifier NewChangeNotifier(EventHandler<ADObjectChangedEventArgs> handler)
        {
            ChangeNotifier cn = new ChangeNotifier(_adConfig);

            cn.ADObjectChanged += new EventHandler<ADObjectChangedEventArgs>(handler);
            cn.Register();

            return cn;
        }

        // -----------------------------------------------------------------------------
        public void DisposeChangeNotifier(ref ChangeNotifier cn)
        {
            if (cn != null)
            {
                cn.Dispose();
                cn = null;
            }
        }
    }
}