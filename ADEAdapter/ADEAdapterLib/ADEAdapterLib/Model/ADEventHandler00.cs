using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Utility;

using ADEAdapterLib.Configuration;

namespace ADEAdapterLib.Model
{
    // ================================================================================
    /// <summary>
    /// Default / sample ADEvent eventhandler.
    /// </summary>
    public class ADEventHandler00 : IADEventHandler
    {
        readonly IADEAConfig _config;
        readonly GK.AD.DTO.ADEvent _adEvent;

        // -----------------------------------------------------------------------------
        public ADEventHandler00(IADEAConfig config, GK.AD.DTO.ADEvent adEvent)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _adEvent = Verify.ArgumentNotNull(adEvent, "adEvent");
        }

        // -----------------------------------------------------------------------------
        public void HandleEvent()
        {
            if (_config.LogReceivedNotifications)
            {
                _config.Logger.LogINFO(string.Format("EVENT RECEIVED IN HANDLER: [{0}]", _adEvent.ToString()));
            }

            try
            {
                // Gør et eller andet ...
            }

            catch (Exception ex)
            {
                if (false)
                {
                    // Fejlen er PERMANENT, ... skriv i log eller lign. ...
                    // Kast IKKE fejlen videre ...
                }
                else
                {
                    // Fejlen er FORBIGÅENDE, ... skriv i log eller lign. ...
                    // Kast fejlen videre ...
                    throw ex;
                }
            }
        }
    }
}
