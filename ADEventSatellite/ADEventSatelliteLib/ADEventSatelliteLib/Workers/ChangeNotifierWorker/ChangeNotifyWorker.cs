using ADEventSatellite.Configuration;
using ADEventSatellite.Model;

using GK.AD;
using GK.AppCore.Queues;
using GK.AppCore.Threads;
using GK.AppCore.Utility;

using System;
using System.DirectoryServices.Protocols;

namespace ADEventSatellite.Workers
{
    // ================================================================================
    public class ChangeNotifyWorker : DefaultWorker
    {
        readonly object _lock = new object();

        readonly IADESLConfig _config;

        readonly IChangeNotifierAgent _changeNotifierAgent;
        readonly IMessageHandler _messageHandler;

        ChangeNotifier _changeNotifier01 = null;
        ChangeNotifier _changeNotifier02 = null;

        int _changeNotifierLifetime = 60 * 1000;
        bool _logChangeNotifierShifts = false;

        bool _inFailedLDAPState = false;

        // -----------------------------------------------------------------------------
        public ChangeNotifyWorker(IADESLConfig config, IChangeNotifierAgent changeNotifierAgent, IMessageHandler messageHandler)
        {
            _config = Verify.ArgumentNotNull(config, "config");

            _changeNotifierAgent = Verify.ArgumentNotNull(changeNotifierAgent, "changeNotifierAgent");
            _messageHandler = Verify.ArgumentNotNull(messageHandler, "messageHandler");
        }

        // -----------------------------------------------------------------------------
        public override void Start()
        {
            _changeNotifierLifetime = _config.ChangeNotifierLifetimeInSec * 1000;
            _logChangeNotifierShifts = _config.LogChangeNotifierShifts;

            _inFailedLDAPState = false;

            lock (_lock) { _messageHandler.Init(); }
            base.Start();
        }

        // -----------------------------------------------------------------------------
        public override void Stop()
        {
            base.Stop();

            lock (_lock)
            {
                _changeNotifierAgent.DisposeChangeNotifier(ref _changeNotifier01);
                _changeNotifierAgent.DisposeChangeNotifier(ref _changeNotifier02);

                _messageHandler.CleanupOnExit();
            }
        }

        // -----------------------------------------------------------------------------
        protected override int ExecutePieceOfWork()
        {
            try
            {
                lock (_lock)
                {
                    if (_inFailedLDAPState)
                    {
                        _messageHandler.Init();
                        _inFailedLDAPState = false;
                    }
                }

                if (_logChangeNotifierShifts) _config.Logger.LogINFO("SHIFT into new AD change notifier...");

                if (_changeNotifier01 == null)
                {
                    _changeNotifier01 = _changeNotifierAgent.NewChangeNotifier(ADObjectChanged);
                    _changeNotifierAgent.DisposeChangeNotifier(ref _changeNotifier02);
                }
                else
                {
                    _changeNotifier02 = _changeNotifierAgent.NewChangeNotifier(ADObjectChanged);
                    _changeNotifierAgent.DisposeChangeNotifier(ref _changeNotifier01);
                }
            }
            catch (LdapException ex)
            {
                string extmsg = ex.Message.Contains("credential is invalid") ? "Verify that application settings ADSysUsername/ADSysUserPswd in (*.config) is either valid, empty or undefined. In the latter two cases AD will be accessed in the context of the current process account. " : "";
                string errmsg = string.Format("Inside ChangeNotifyWorker.ExecutePieceOfWork(), LDAP exception=[{0}]. {1}", ex.Message, extmsg);

                _config.Logger.LogERROR(errmsg);

                lock (_lock)
                {
                    _changeNotifierAgent.DisposeChangeNotifier(ref _changeNotifier01);
                    _changeNotifierAgent.DisposeChangeNotifier(ref _changeNotifier02);

                    _messageHandler.CleanupOnExit();

                    _inFailedLDAPState = true;
                }

                // In case of errors, a new notifier is created faster (5 secs) than normal ...
                return 5 * 1000;
            }
            catch (Exception ex)
            {
                _config.Logger.LogERROR("Inside ChangeNotifyWorker.ExecutePieceOfWork(), Ex=[" + ex.Message + "]. ");

                // In case of errors, a new notifier is created faster (5 secs) than normal ...
                return 5 * 1000;
            }

            return _changeNotifierLifetime;
        }

        #region private methods

        // -----------------------------------------------------------------------------
        void ADObjectChanged(object sender, ADObjectChangedEventArgs e)
        {
            _messageHandler.HandleMessage(e);
        }

        #endregion private methods
    }
}