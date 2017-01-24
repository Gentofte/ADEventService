using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Utility;

namespace ADEventService
{
    // ================================================================================
    partial class ADEventServiceMain : ServiceBase
    {
        readonly IServiceControl _runner;

        // -----------------------------------------------------------------------------
        public ADEventServiceMain()
        {
            InitializeComponent();
        }

        // -----------------------------------------------------------------------------
        public ADEventServiceMain(IServiceControl runner)
        {
            InitializeComponent();

            _runner = Verify.ArgumentNotNull(runner, "runner");
        }

        // -----------------------------------------------------------------------------
        protected override void OnStart(string[] args)
        {
            Start1();
        }

        // -----------------------------------------------------------------------------
        protected override void OnStop()
        {
            Stop1();
        }

        // -----------------------------------------------------------------------------
        void Start1()
        {
            try
            {
                if (_runner != null)
                {
                    _runner.Init();
                    _runner.Start();
                }
            }
            catch (Exception ex)
            {
                HaltOperation(ex, "START");
            }
        }

        // -----------------------------------------------------------------------------
        void Stop1()
        {
            try
            {
                if (_runner != null)
                {
                    _runner.Stop();
                    _runner.CleanupOnExit();
                }
            }
            catch (Exception ex)
            {
                HaltOperation(ex, "STOP");
            }
        }

        // -----------------------------------------------------------------------------
        void HaltOperation(Exception ex, string auxMessage)
        {
            string msg = string.Format("Inside ADEventServiceMain.HaltOperation()/{0}: [{1}]", auxMessage, ex.Message);
            EventLog.WriteEntry(msg, EventLogEntryType.Error, 999);

            Environment.Exit(1);
        }
    }
}
