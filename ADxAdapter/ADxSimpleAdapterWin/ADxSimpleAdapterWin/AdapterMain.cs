using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Owin.Hosting;

using ADxSimpleAdapterWin.Configuration;

namespace ADxSimpleAdapterWin
{
    // ================================================================================
    public partial class AdapterMain : Form
    {
        readonly IADxConfig _config = null;

        IDisposable _webapp = null;

        // -----------------------------------------------------------------------------
        public AdapterMain()
        {
            InitializeComponent();
        }

        // -----------------------------------------------------------------------------
        public AdapterMain(IADxConfig config)
        {
            InitializeComponent();

            _config = config;
            _config.Logger.SetLogControl(rtbLog);
        }

        // -----------------------------------------------------------------------------
        void StartEngineController()
        {
            var uri = new Uri(_config.ServiceBaseURL);
            var baseUri = string.Format("http://+:{0}", uri.Port.ToString());

            if (_webapp == null) _webapp = WebApp.Start<Startup>(baseUri);

            _config.Logger.LogINFO("Ready...");
        }

        // -----------------------------------------------------------------------------
        void StopEngineController()
        {
            if (_webapp != null) _webapp.Dispose();
        }

        // -----------------------------------------------------------------------------
        private void AdapterMain_Load(object sender, EventArgs e)
        {
            StartEngineController();
        }

        // -----------------------------------------------------------------------------
        private void AdapterMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopEngineController();
        }

        // -----------------------------------------------------------------------------
        private void btnClear_Click(object sender, EventArgs e)
        {
            _config.Logger.ClearLog();
        }
    }
}
