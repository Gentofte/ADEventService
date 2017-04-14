using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;

using ADxSimpleAdapterWin.Model;

namespace ADxSimpleAdapterWin.Configuration
{
    // ================================================================================
    public class IoCConfig
    {
        // -----------------------------------------------------------------------------
        public void ConfigureIoCStuff(IUnityContainer container)
        {
            container.RegisterType<ILog, Log>(new ContainerControlledLifetimeManager());

            container.RegisterType<IADxConfig, ADxConfig>(new ContainerControlledLifetimeManager());

            container.RegisterType<IEngine, Engine>(new ContainerControlledLifetimeManager());
        }

    }
}
