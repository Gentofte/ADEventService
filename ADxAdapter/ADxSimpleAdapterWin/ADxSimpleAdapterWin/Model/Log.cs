using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADxSimpleAdapterWin.Model
{
    // ================================================================================
    public class Log : ILog
    {
        ADxSimpleAdapterWin.Forms.SafeRTF _rtbControl = null;
        int _msgCount = 0;

        // -----------------------------------------------------------------------------
        public void SetLogControl(ADxSimpleAdapterWin.Forms.SafeRTF rtbControl)
        {
            _rtbControl = rtbControl;
            ClearLog();
        }

        // -----------------------------------------------------------------------------
        public void ClearLog()
        {
            _rtbControl.Clear();
            _msgCount = 0;
        }

        // -----------------------------------------------------------------------------
        public void LogINFO(string message)
        {
            _rtbControl.AppendText(string.Format("{0} : {1}", _msgCount.ToString("00000"), message));
            _msgCount++;
        }
    }
}
