using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GK.AppCore.Logging;
using GK.AppCore.Queues;
using GK.AppCore.Threads;

using Microsoft.Practices.Unity;

using ADEAdapterLib.Model;
using ADEAdapterLib.Workers;

namespace ADEAdapterLib.Configuration
{
    // ================================================================================
    public class IoCConfig
    {
        // -----------------------------------------------------------------------------
        public void ConfigureIoCStuff(IUnityContainer container)
        {
            // External library IoC mappings follows ==>

            (new GK.AppCore.Configuration.IoCConfig()).ConfigureIoCStuff(container);
            (new GK.AD.Configuration.IoCConfig()).ConfigureIoCStuff(container);
            (new GK.AD.DTO.Configuration.IoCConfig()).ConfigureIoCStuff(container);
            (new GK.AD.MAP.Configuration.IoCConfig()).ConfigureIoCStuff(container);

            // INTERNAL library IoC mappings follows ==>

            container.RegisterType<IADEAConfig, ADEAConfig>(new ContainerControlledLifetimeManager());

            container.RegisterType<IServiceControl, ServiceControl>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEngine, Engine>(new ContainerControlledLifetimeManager());

            // Register queue publishers
            container.RegisterType<IIncomingEventQueuePublisher, IncomingEventQueuePublisher>(new ContainerControlledLifetimeManager());

            // Register worker threads ...
            container.RegisterType<IWorker, HousemateWorker>("HousemateWorker");

            // Register incoming queue
            ConfigureIoCStuff_IncomingEventWorker(container);

            // Register event default event handler factory. OVERRIDE THIS REGISTRATION IN THE LIBRARY DOING THE REAL ADAPTER WORK
            container.RegisterType<IADEventHandlerFactory, ADEventHandlerFactory00>(new ContainerControlledLifetimeManager());
        }

        // -----------------------------------------------------------------------------
        void ConfigureIoCStuff_IncomingEventWorker(IUnityContainer container)
        {
            container.RegisterType<IMessageHandler, IncomingEventQueueMessageHandler>("IncomingEventQueueMessageHandler");
            container.RegisterType<IWorker, IncomingEventWorker>("IncomingEventWorker",
                new InjectionConstructor(
                    new ResolvedParameter<IADEAConfig>(),
                    new ResolvedParameter<IQueueFactory>(),
                    new ResolvedParameter<IMessageHandler>("IncomingEventQueueMessageHandler")
                    ));
        }
    }
}
