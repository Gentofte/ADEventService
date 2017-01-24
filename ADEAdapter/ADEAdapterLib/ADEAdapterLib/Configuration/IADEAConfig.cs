using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Configuration;
using GK.AppCore.Logging;

namespace ADEAdapterLib.Configuration
{
    // ================================================================================
    public interface IADEAConfig
    {
        // -----------------------------------------------------------------------------
        ILog Logger { get; }

        // -----------------------------------------------------------------------------
        string ServiceBaseURL { get; }

        // -----------------------------------------------------------------------------
        Guid ApplicationID { get; }

        // -----------------------------------------------------------------------------
        string ApplicationPrefix { get; }

        // -----------------------------------------------------------------------------
        bool IsProductionEnvironment { get; }

        // -----------------------------------------------------------------------------
        bool EchoHousemateVisits { get; }

        // -----------------------------------------------------------------------------
        bool LogReceivedNotifications { get; }

        // -----------------------------------------------------------------------------
        string IncomingEventExchange { get; }

        // -----------------------------------------------------------------------------
        string IncomingEventQueue { get; }


    }
}
