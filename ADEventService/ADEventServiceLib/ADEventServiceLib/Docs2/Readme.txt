

netsh http add urlacl url=http://+:8300/ user=%USERNAME%


===

<appender name="RollingLogFileAppender" type="log4net.Appender.RollingFileAppender">
    <file value="logfile" />
    <appendToFile value="true" />
    <rollingStyle value="Composite" />
    <datePattern value="yyyyMMdd" />
    <maxSizeRollBackups value="10" />
    <maximumFileSize value="1MB" />
    <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" />
    </layout>
</appender>

<staticLogFileName value="true" />


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;

using ADChangeTracker.Config;
using ADChangeTracker.Model;
using ADChangeTracker.Workers;

using GK.AppCore.Config;
using GK.AppCore.EntryPoint;
using GK.AppCore.Forms;
using GK.AppCore.Logging;
using GK.AppCore.Mail;
using GK.AppCore.Threads;

using GK.AppCore.Error;

using Microsoft.Practices.Unity;

namespace ADChangeTracker.EntryPoint
{
    // ================================================================================
    public class MyEntryPoint : IEntryPoint
    {
        readonly IUnityContainer _container = new UnityContainer();
        readonly ITextBox _textBox = null;

        IMyConfig _config = null;
        IWorkerCollection _workerCollection = null;

        // -----------------------------------------------------------------------------
        public IMyConfig MyConfig
        { get { return _config; } }

        // -----------------------------------------------------------------------------
        public MyEntryPoint(ITextBox outputTextBox)
        {
            _textBox = outputTextBox;
        }

        // -----------------------------------------------------------------------------
        public void Init()
        {
            // Register dependencies
            Register();

            // Explict resolve main dependencies

            // Make sure that singleton config object is instantiated
            _config = _container.Resolve<IMyConfig>();

            _workerCollection = _container.Resolve<IWorkerCollection>("DefaultWorkerCollection");
            _workerCollection.Add(_container.Resolve<IWorker>("ChangeNotifyWorker"));
            _workerCollection.Add(_container.Resolve<IWorker>("RawEventWorker"));
        }

        // -----------------------------------------------------------------------------
        public void Start()
        {
            if (_workerCollection != null) _workerCollection.Start();

            OutputConfigParams();
        }

        // -----------------------------------------------------------------------------
        public void Stop()
        {
            if (_workerCollection != null) _workerCollection.Stop();
        }

        // -----------------------------------------------------------------------------
        public bool IsRunning()
        {
            if (_workerCollection != null) return _workerCollection.IsRunning;

            return false;
        }

        // -----------------------------------------------------------------------------
        public void CleanupOnExit()
        {
        }

        // -----------------------------------------------------------------------------
        void Register()
        {
            // 1) Log config objects ...
            if (_textBox != null)
            {
                _container.RegisterType<ILog, DefaultGUILog>(
                    "DefaultGUILog",
                    new InjectionFactory(c => new DefaultGUILog(_textBox))
                    );
            }

            //
            

            _container.RegisterType<ILog, DefaultFileLog>("DefaultFileLog");
            //_container.RegisterType<ILog, DefaultMQLog>("DefaultMQLog", new ContainerControlledLifetimeManager());

            _container.RegisterType<IEnumerable<ILog>, ILog[]>();
            _container.RegisterType<ILog, LogCollection>();

            // ---

            // 2) Mail config objects ...
            _container.RegisterType<IMailConfig, DefaultMailConfig>();

            // 3) Core application config object
            _container.RegisterType<IAppConfig, DefaultAppConfig>(new ContainerControlledLifetimeManager());

            // x) Appl specific config object
            _container.RegisterType<IMyConfig, MyConfig>(new ContainerControlledLifetimeManager());

            // x) Register curcuit breaker. TODO move to core GK.AppCore
            _container.RegisterType<ICircuitBreaker, DefaultCircuitBreaker>();

            // x) Register ...
            _container.RegisterType<INotifierDropFilter, UserGroupOUFilter>();

            RegisterChangeNotifierAgentStuff();

            // x) Send raw event registrations including circuit breaker and exception handling decorators
            RegisterSendRawEventAgentStuff();

            //
            _container.RegisterType<IWorkerCollection, DefaultWorkerCollection>("DefaultWorkerCollection");
            _container.RegisterType<IWorker, ChangeNotifyWorker>("ChangeNotifyWorker");
            _container.RegisterType<IWorker, RawEventWorker>("RawEventWorker");

            //_container.RegisterType<IWorker, MQLogTraceWorker>("MQLogTraceWorker");

        }

        // -----------------------------------------------------------------------------
        void RegisterChangeNotifierAgentStuff()
        {
            _container.RegisterType<IChangeNotifierAgent, CircuitBreakerChangeNotifierAgent>
                (new InjectionConstructor(new ResolvedParameter<ChangeNotifierAgent>(), new ResolvedParameter<ICircuitBreaker>())
                );

            _container.RegisterType<IChangeNotifierAgent, ExceptionHandlerChangeNotifierAgent>
                (new InjectionConstructor(new ResolvedParameter<CircuitBreakerChangeNotifierAgent>())
                );
        }

        // -----------------------------------------------------------------------------
        void RegisterSendRawEventAgentStuff()
        {
            //_container.RegisterType<ISendRawEventAgent, SendRawEventAgent>("SendRawEventAgent");
            //_container.RegisterType<ISendRawEventAgent, CurcuitBreakerSendRawEventAgent>
            //    ("CurcuitBreakerSendRawEventAgent", new InjectionConstructor(new ResolvedParameter<SendRawEventAgent>("SendRawEventAgent"), new ResolvedParameter<ICurcuitBreaker>())
            //    );

            //_container.RegisterType<ISendRawEventAgent, ExceptionHandlerSendRawEventAgent>
            //    (new InjectionConstructor(new ResolvedParameter<ISendRawEventAgent>("CurcuitBreakerSendRawEventAgent"))
            //    );


            //_container.RegisterType<ISendRawEventAgent, SendRawEventAgent>();

            _container.RegisterType<ISendRawEventAgent, CircuitBreakerSendRawEventAgent>
                (new InjectionConstructor(new ResolvedParameter<SendRawEventAgent>(), new ResolvedParameter<ICircuitBreaker>())
                );

            _container.RegisterType<ISendRawEventAgent, ExceptionHandlerSendRawEventAgent>
                (new InjectionConstructor(new ResolvedParameter<CircuitBreakerSendRawEventAgent>())
                );
        }

        // -----------------------------------------------------------------------------
        void OutputConfigParams()
        {
            _config.AppConfig.Log.LogWRITE("");
            _config.AppConfig.Log.LogWRITE("---->>  CONFIGURATION  <<----");

            string notifierLifetime = "AD change notifer is renewed every (" + _config.ChangeNotifierLifetime.ToString() + ") seconds";
            _config.AppConfig.Log.LogINFO(notifierLifetime);

            string eventQueueName = "AD events is stored in (RabbitMQ) queue=[ " + _config.ADRawEventQueue + " ]";
            _config.AppConfig.Log.LogINFO(eventQueueName);

            _config.AppConfig.Log.LogWRITE("----");
        }

    }
}
