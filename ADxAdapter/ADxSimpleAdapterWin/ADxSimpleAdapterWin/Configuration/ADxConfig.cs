using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADxSimpleAdapterWin.Model;

namespace ADxSimpleAdapterWin.Configuration
{
    // ================================================================================
    public class ADxConfig : IADxConfig
    {
        readonly ILog _log;

        // The following three settings is used to identify the subscription for the
        // sample adapter at the ADEventService if the EnableADxSampleSubscription setting in
        // ADEventService app.config in turned ON. DONT CHANGE!
        readonly string _serviceBaseURL = "http://localhost:8810";
        readonly Guid _applID = new Guid("18BEAC30-8FE5-437C-8D79-404692AB48D1");
        readonly string _applPrefix = "ADxSAW-01";

        // -----------------------------------------------------------------------------
        public ADxConfig(ILog log)
        {
            _log = log;
        }

        // -----------------------------------------------------------------------------
        public bool LogReceivedNotifications { get { return true; } }

        // -----------------------------------------------------------------------------
        public ILog Logger { get { return _log; } }

        // -----------------------------------------------------------------------------
        public string ServiceBaseURL { get { return _serviceBaseURL; } }

        // -----------------------------------------------------------------------------
        public Guid ApplicationID { get { return _applID; } }

        // -----------------------------------------------------------------------------
        public string ApplicationPrefix { get { return _applPrefix; } }
    }
}
