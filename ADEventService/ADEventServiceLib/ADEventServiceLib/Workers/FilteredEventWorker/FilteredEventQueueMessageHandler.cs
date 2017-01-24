using System;

using GK.AD;
using GK.AD.Events;

using GK.AppCore.Queues;
using GK.AppCore.Utility;

using ADEventSatellite.DTO;

using ADEventService.Configuration;
using ADEventService.Models;

namespace ADEventService.Workers
{
    // ================================================================================
    public class FilteredEventQueueMessageHandler : IMessageHandler
    {
        readonly IADESConfig _config;
        readonly IRawEventFilter _eventFilter;
        readonly IFilteredEventQueuePublisher _publisher;

        // -----------------------------------------------------------------------------
        public FilteredEventQueueMessageHandler(IADESConfig config, IRawEventFilter eventFilter, IFilteredEventQueuePublisher publisher)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _eventFilter = Verify.ArgumentNotNull(eventFilter, "eventFilter");
            _publisher = Verify.ArgumentNotNull(publisher, "publisher");
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
            _eventFilter.Init();
        }

        // -----------------------------------------------------------------------------
        public void CleanupOnExit()
        {
            _eventFilter.CleanupOnExit();
        }

        // -----------------------------------------------------------------------------
        public void HandleMessage(byte[] messageBody)
        {
            try
            {
                var eventPackage = Serializer.DeSerializeFromByteArray<EventPackage>(messageBody);
                var rawEvent = Convert.FromBase64String(eventPackage.ADEventAsB64);
                var adEvent = Serializer.DeSerializeFromByteArray<ADEvent>(rawEvent);

                IADObject adObj = _eventFilter.FilterObject(adEvent.ADObject, adEvent.Sender.When);

                if (adObj != null)
                {
                    _publisher.PublishMessage(messageBody);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }
        }

        // -----------------------------------------------------------------------------
        public void HandleMessage(object messageBody) { throw new NotImplementedException(); }

        // -----------------------------------------------------------------------------
        void Trace(string s)
        {
            if (true)
            {
                _config.Logger.LogINFO(s);
            }
        }
    }
}
