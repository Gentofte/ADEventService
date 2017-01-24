using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Practices.Unity;

using GK.AppCore.Logging;
using GK.AppCore.Unity;
using GK.AppCore.Utility;

namespace ADEventSatelliteWin
{
    // ================================================================================
    static class Program
    {
        // -----------------------------------------------------------------------------
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ApplicationExit += Application_ApplicationExit;

            IUnityContainer container = UnityConfig.GetConfiguredContainer();
            DoIoCConfiguration(container);

            ADEventSatelliteMain mainForm = container.Resolve<ADEventSatelliteMain>();

            mainForm.Init();

            Application.Run(mainForm);
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
            (new ADEventSatellite.Configuration.IoCConfig()).ConfigureIoCStuff(container);
        }

    }
}
