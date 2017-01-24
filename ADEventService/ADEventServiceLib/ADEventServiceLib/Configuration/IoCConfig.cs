using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GK.AppCore.Logging;
using GK.AppCore.Queues;
using GK.AppCore.Threads;

using Microsoft.Practices.Unity;

using ADEventService.MAP;
using ADEventService.Models;
using ADEventService.Workers;

namespace ADEventService.Configuration
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
            (new GK.AD.MAP.Configuration.IoCConfig()).ConfigureIoCStuff(container);
            //(new ADEventSatellite.Configuration.IoCConfig()).ConfigureIoCStuff(container);

            // INTERNAL library IoC mappings follows ==>

            container.RegisterType<IADESConfig, ADESConfig>(new ContainerControlledLifetimeManager());
            container.RegisterType<IServiceControl, ServiceControl>(new ContainerControlledLifetimeManager());
            container.RegisterType<IADESEngine, ADESEngine>(new ContainerControlledLifetimeManager());

            // Register subscription handling ...
            ConfigureIoC_SubscriptionStuff(container);

            // Register queue publishers
            container.RegisterType<IRawEventQueuePublisher, RawEventQueuePublisher>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFilteredEventQueuePublisher, FilteredEventQueuePublisher>(new ContainerControlledLifetimeManager());


            // Register worker threads ...
            container.RegisterType<IWorker, HousemateWorker>("HousemateWorker");

            ConfigureIoC_RawEventWorker(container);

            ConfigureIoC_SubscriptionWorker(container);
        }

        // -----------------------------------------------------------------------------
        void ConfigureIoC_SubscriptionStuff(IUnityContainer container)
        {
            container.RegisterType<ISubscription, Subscription>();
            container.RegisterType<ICreateSubscriptionRequest, CreateSubscriptionRequest>();
            container.RegisterType<ISubscriptionRepo, SubscriptionRepo>(new ContainerControlledLifetimeManager());

            container.RegisterType<INullSubscription, NullSubscription>();

            container.RegisterType<ISubscriptionMapper, SubscriptionMapper>(new ContainerControlledLifetimeManager());
        }

        // -----------------------------------------------------------------------------
        void ConfigureIoC_RawEventWorker(IUnityContainer container)
        {
            container.RegisterType<IRawEventFilter, RedisRawEventFilter>(new ContainerControlledLifetimeManager());

            container.RegisterType<IMessageHandler, RawEventQueueMessageHandler>("RawEventQueueMessageHandler");
            container.RegisterType<IWorker, RawEventWorker>("RawEventWorker",
                new InjectionConstructor(
                    new ResolvedParameter<IADESConfig>(),
                    new ResolvedParameter<IQueueFactory>(),
                    new ResolvedParameter<IMessageHandler>("RawEventQueueMessageHandler")
                    ));
        }

        // -----------------------------------------------------------------------------
        void ConfigureIoC_SubscriptionWorker(IUnityContainer container)
        {
            container.RegisterType<ISubscriptionEventWorkerFactory, SubscriptionEventWorkerFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISubscribtionEventQueueMessageHandlerFactory, SubscribtionEventQueueMessageHandlerFactory>(new ContainerControlledLifetimeManager());

            //container.RegisterType<IMessageHandler, RawEventQueueMessageHandler>("RawEventQueueMessageHandler");
            //container.RegisterType<IWorker, SubscriptionWorker>("SubscriptionWorker",
            //    new InjectionConstructor(
            //        new ResolvedParameter<IADESConfig>(),
            //        new ResolvedParameter<IQueueFactory>(),
            //        new ResolvedParameter<IMessageHandler>("RawEventQueueMessageHandler")
            //        ));
        }
    }
}
