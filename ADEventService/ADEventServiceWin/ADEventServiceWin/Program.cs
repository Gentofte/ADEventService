using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Practices.Unity;

using GK.AppCore.Logging;
using GK.AppCore.Unity;
using GK.AppCore.Utility;

using ADEventService.Configuration;

namespace ADEventServiceWin
{
    // ================================================================================
    static class Program
    {
        // -----------------------------------------------------------------------------
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ApplicationExit += Application_ApplicationExit;

            IUnityContainer container = UnityConfig.GetConfiguredContainer();
            DoIoCConfiguration(container);

            ADEventServiceMain mainForm = container.Resolve<ADEventServiceMain>();

            mainForm.Init();

            var reset = ResetParamSpecified(args);

            if (reset)
            {
                var config = container.Resolve<IADESConfig>();
                config.Reset();

                //Environment.Exit(0);
            }

            Application.Run(mainForm);
        }

        // -----------------------------------------------------------------------------
        static bool ResetParamSpecified(string[] args)
        {
            if (args.Length > 0)
            {
                var arg0 = args[0];
                arg0 = string.IsNullOrWhiteSpace(arg0) ? "" : arg0.Trim().ToLowerInvariant();

                return (arg0 == "--reset");
            }

            return false;
        }

        // -----------------------------------------------------------------------------
        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        // -----------------------------------------------------------------------------
        static void DoIoCConfiguration(IUnityContainer container)
        {
            (new GK.AppCore.Configuration.IoCConfig()).ConfigureIoCStuff(container);
            (new ADEventService.Configuration.IoCConfig()).ConfigureIoCStuff(container);
        }

    }
}
