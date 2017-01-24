using System;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;

using Microsoft.Practices.Unity;

using GK.AppCore.Unity;

using ADEventService.Configuration;

namespace ADEventService
{
    // ================================================================================
    static class Program
    {
        // -----------------------------------------------------------------------------
        static void Main(string[] args)
        {
            if (System.Environment.UserInteractive)
            {
                if (args == null) args = new string[0];
                var parm = string.Concat(args).ToLowerInvariant();

                switch (parm)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--reset":
                        {
                            IUnityContainer container = UnityConfig.GetConfiguredContainer();
                            DoIoCConfiguration(container);

                            var config = container.Resolve<IADESConfig>();
                            config.Reset();

                            //Environment.Exit(0);
                        }
                        break;
                }
            }
            else
            {
                IUnityContainer container = UnityConfig.GetConfiguredContainer();
                DoIoCConfiguration(container);

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { container.Resolve<ADEventServiceMain>() };
                ServiceBase.Run(ServicesToRun);
            }
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
