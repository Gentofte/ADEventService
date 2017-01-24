using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ADEventSatellite;

namespace ADEventSatelliteWin
{
    // ================================================================================
    public partial class ADEventSatelliteMain : GK.AppCore.Forms.ServiceConsoleBaseRTF
    {
        // -----------------------------------------------------------------------------
        public ADEventSatelliteMain()
        {
            InitializeComponent();
        }

        // -----------------------------------------------------------------------------
        public ADEventSatelliteMain(IServiceControl runner)
            : base(runner)
        {
            InitializeComponent();
        }
    }
}
