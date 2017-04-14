using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADxSimpleAdapterWin.Model
{
    // ================================================================================
    public interface ILog
    {
        // -----------------------------------------------------------------------------
        void SetLogControl(ADxSimpleAdapterWin.Forms.SafeRTF rtbControl);

        // -----------------------------------------------------------------------------
        void ClearLog();

        // -----------------------------------------------------------------------------
        void LogINFO(string message);
    }
}
