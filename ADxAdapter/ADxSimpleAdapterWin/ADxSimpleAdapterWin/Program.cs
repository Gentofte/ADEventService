using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Practices.Unity;

using ADxSimpleAdapterWin.Unity;

namespace ADxSimpleAdapterWin
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
            mainForm.Text = "ADxSimpleAdapterWin";

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
            (new ADxSimpleAdapterWin.Configuration.IoCConfig()).ConfigureIoCStuff(container);
            (new GK.AD.DTO.Configuration.IoCConfig()).ConfigureIoCStuff(container);
        }

    }
}
