using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ADEventService;

namespace ADEventServiceWin
{
    // ================================================================================
    public partial class ADEventServiceMain : GK.AppCore.Forms.ServiceConsoleBaseRTF
    {
        // -----------------------------------------------------------------------------
        public ADEventServiceMain()
        {
            InitializeComponent();
        }

        // -----------------------------------------------------------------------------
        public ADEventServiceMain(IServiceControl runner)
            : base(runner)
        {
            InitializeComponent();
        }
    }
}
