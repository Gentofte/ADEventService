using ADEventSatellite.Configuration;
using ADEventSatellite.Model;

using GK.AD;
using GK.AD.Events;
using GK.AppCore.Queues;
using GK.AppCore.Utility;
using System;

namespace ADEventSatellite.Workers
{
    // ================================================================================
    /// <summary>
    /// This class handles notifications from AD. When a notification is recevied it's
    /// converted to a raw event and stored in the raw event queue.
    /// </summary>
    public class ChangeNotifyMessageHandler : IMessageHandler
    {
        readonly IADESLConfig _config;
        readonly IADObjectFactory _adObjectFactory;
        readonly INotifierDropFilter _dropFilter;
        readonly IADEventFactory _adEventFactory;
        readonly IChangeNotifyEventQueuePublisher _publisher;

        readonly IDuplicateFilter _duplicateFilter;

        // -----------------------------------------------------------------------------
        public ChangeNotifyMessageHandler(IADESLConfig config, IADObjectFactory adObjectFactory, INotifierDropFilter dropFilter, IADEventFactory adEventFactory, IChangeNotifyEventQueuePublisher publisher, IDuplicateFilter duplicateFilter)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _adObjectFactory = Verify.ArgumentNotNull(adObjectFactory, "adObjectFactory");
            _dropFilter = Verify.ArgumentNotNull(dropFilter, "dropFilter");
            _adEventFactory = Verify.ArgumentNotNull(adEventFactory, "adEventFactory");
            _publisher = Verify.ArgumentNotNull(publisher, "publisher");

            _duplicateFilter = Verify.ArgumentNotNull(duplicateFilter, "duplicateFilter");
        }

        // -----------------------------------------------------------------------------
        public event EventHandler<MessageDoneEventArgs> MessageDone
        {
            add { }
            remove { }
        }

        // -----------------------------------------------------------------------------
        public void Init()
        {
            _publisher.Init();
            _duplicateFilter.Init();
        }

        // -----------------------------------------------------------------------------
        public void CleanupOnExit()
        {
            _publisher.CleanupOnExit();
            _duplicateFilter.CleanupOnExit();
        }

        // -----------------------------------------------------------------------------
        public void HandleMessage(byte[] messageBody)
        {
            throw new NotImplementedException();
        }

        // -----------------------------------------------------------------------------
        public void HandleMessage(object messageBody)
        {
            ADObjectChangedEventArgs eventArg = null;

            try
            {
                eventArg = (ADObjectChangedEventArgs)messageBody;

                Guid objectGuid = GK.AD.Utility.Extract_objectGuid(eventArg.Result.Attributes);
                bool isDeleted = GK.AD.Utility.Extract_isDeleted(eventArg.Result.Attributes);

                IADObject adObj = isDeleted ?
                    _adObjectFactory.GetDeletedObjectFromID(objectGuid.ToString()) :
                    _adObjectFactory.GetObjectFromID(objectGuid.ToString());

                // Filter out any object other than user, OU and group (depending on filter used)
                adObj = _dropFilter.FilterObject(adObj);

                // Filter out any duplicates ...
                adObj = _duplicateFilter.FilterObject(adObj);

                if (adObj != null)
                {
                    ADEvent adEvent = _adEventFactory.CreateADEvent(adObj);

                    // Enable FAKE errors in DEV/TEST env.
                    if (_config.IsProductionEnvironment == false)
                    {
                        if (adObj.description == "FAKE-ERROR") adEvent.Sender.What = "FAKE-ERROR";
                        if (adObj.description == "TIMEOUT") adEvent.Sender.What = "TIMEOUT";
                    }

                    byte[] body = _adEventFactory.Serialize(adEvent);

                    _publisher.PublishMessage(body);

                    LogNotificationReceivedEvent(adObj);
                }
            }
            catch (Exception ex)
            {
                string objDN = (eventArg == null || eventArg.Result == null || eventArg.Result.DistinguishedName == null) ? "NULL" : eventArg.Result.DistinguishedName;
                _config.Logger.LogERROR(string.Format("Exception [{3}] occurred inside {0}, DN=>[{1}], Ex=[{2}]. AD CHANGE (notification) is LOST! ", ExeInfo.GetCurrentMethod(true).SmartTruncate(), objDN, ex.Message, ex.GetType().ToString()));
            }
        }

        // -----------------------------------------------------------------------------
        void LogNotificationReceivedEvent(IADObject adObj)
        {
            if (_config.LogNotificationEvents)
                _config.Logger.LogINFO(string.Format("AD CHANGE RECEIVED: [{0}]", adObj.ToString()));
        }
    }
}