using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Practices.Unity;

using GK.AppCore.Unity;

namespace ADEAdapterWin
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

            AdapterMain mainForm = container.Resolve<AdapterMain>();

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
            // Always include config of the following
            (new GK.AppCore.Configuration.IoCConfig()).ConfigureIoCStuff(container);
            (new ADEAdapterLib.Configuration.IoCConfig()).ConfigureIoCStuff(container);

            // Include 3. party config ...
            //(new xxxAdapterLib.Configuration.IoCConfig()).ConfigureIoCStuff(container);
        }

    }
}
