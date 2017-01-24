using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ADEAdapterLib;

namespace ADEAdapterWin
{
    // ================================================================================
    public partial class AdapterMain : GK.AppCore.Forms.ServiceConsoleBaseRTF
    {
        // -----------------------------------------------------------------------------
        public AdapterMain()
        {
            InitializeComponent();
        }

        // -----------------------------------------------------------------------------
        public AdapterMain(IServiceControl runner)
            : base(runner)
        {
            InitializeComponent();
        }
    }
}
