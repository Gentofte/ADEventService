using ADEventSatellite.Model;
using ADEventSatellite.Workers;
using GK.AppCore.Configuration;
using GK.AppCore.Queues;
using GK.AppCore.Threads;
using Microsoft.Practices.Unity;

namespace ADEventSatellite.Configuration
{
    // ================================================================================
    public class IoCConfig : IIoCConfig
    {
        object _lock = new object();
        bool _isConfigured = false;

        // -----------------------------------------------------------------------------
        public void ConfigureIoCStuff(IUnityContainer container)
        {
            lock (_lock) { if (_isConfigured) return; _isConfigured = true; }

            // External library IoC mappings follows ==>

            (new GK.AppCore.Configuration.IoCConfig()).ConfigureIoCStuff(container);
            (new GK.AD.Configuration.IoCConfig()).ConfigureIoCStuff(container);

            // INTERNAL library IoC mappings follows ==>

            container.RegisterType<IADESLConfig, ADESLConfig>(new ContainerControlledLifetimeManager());
            container.RegisterType<IServiceControl, ServiceControl>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEngine, Engine>(new ContainerControlledLifetimeManager());

            container.RegisterType<IDuplicateFilter, RedisDuplicateFilter>(new ContainerControlledLifetimeManager());

            // Register queue publishers
            container.RegisterType<IChangeNotifyEventQueuePublisher, ChangeNotifyEventQueuePublisher>(new ContainerControlledLifetimeManager());

            // Register worker threads ...
            container.RegisterType<IWorker, HousemateWorker>("SatelliteHousemateWorker");

            ConfigureIoCStuff_ChangeNotify(container);
            ConfigureIoCStuff_SendEvent2ADEQueueWorker(container);
        }

        // -----------------------------------------------------------------------------
        public bool IsConfigured()
        {
            lock (_lock) { return _isConfigured; }
        }

        // -----------------------------------------------------------------------------
        void ConfigureIoCStuff_ChangeNotify(IUnityContainer container)
        {
            //container.RegisterType<INotifierDropFilter, NoFilter>();
            container.RegisterType<INotifierDropFilter, UserGroupOUFilter>();

            container.RegisterType<IMessageHandler, ChangeNotifyMessageHandler>("ChangeNotifyMessageHandler");
            container.RegisterType<IChangeNotifierAgent, ChangeNotifierAgent>();
            container.RegisterType<IWorker, ChangeNotifyWorker>("ChangeNotifyWorker",
                new InjectionConstructor(
                    new ResolvedParameter<IADESLConfig>(),
                    new ResolvedParameter<IChangeNotifierAgent>(),
                    new ResolvedParameter<IMessageHandler>("ChangeNotifyMessageHandler")
                    ));
        }

        // -----------------------------------------------------------------------------
        void ConfigureIoCStuff_SendEvent2ADEQueueWorker(IUnityContainer container)
        {
            container.RegisterType<IMessageHandler, SendEvent2ADEQueueMessageHandler>("SendEvent2ADEQueueMessageHandler");
            container.RegisterType<IWorker, SendEvent2ADEWorker>("SendEvent2ADEWorker",
                new InjectionConstructor(
                    new ResolvedParameter<IADESLConfig>(),
                    new ResolvedParameter<IQueueFactory>(),
                    new ResolvedParameter<IMessageHandler>("SendEvent2ADEQueueMessageHandler")
                    ));
        }
    }
}