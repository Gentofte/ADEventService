using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADxSimpleAdapterWin.Model;

namespace ADxSimpleAdapterWin.Configuration
{
    // ================================================================================
    public interface IADxConfig
    {
        // -----------------------------------------------------------------------------
        bool LogReceivedNotifications { get; }

        // -----------------------------------------------------------------------------
        ILog Logger { get; }

        // -----------------------------------------------------------------------------
        string ServiceBaseURL { get; }

        // -----------------------------------------------------------------------------
        Guid ApplicationID { get; }

        // -----------------------------------------------------------------------------
        string ApplicationPrefix { get; }

    }
}
