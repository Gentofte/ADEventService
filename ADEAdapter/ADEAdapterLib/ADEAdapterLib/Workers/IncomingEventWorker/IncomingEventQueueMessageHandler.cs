using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Queues;
using GK.AppCore.Utility;
using GK.AppCore.Http;

using ADEAdapterLib.Configuration;
using ADEAdapterLib.Model;

namespace ADEAdapterLib.Workers
{
    // ================================================================================
    public class IncomingEventQueueMessageHandler : IMessageHandler
    {
        readonly IADEAConfig _config;
        readonly IADEventHandlerFactory _eventhandlerFactory;

        // -----------------------------------------------------------------------------
        public IncomingEventQueueMessageHandler(IADEAConfig config, IADEventHandlerFactory eventhandlerFactory)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _eventhandlerFactory = Verify.ArgumentNotNull(eventhandlerFactory, "eventhandlerFactory");
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
        }

        // -----------------------------------------------------------------------------
        public void CleanupOnExit()
        {
        }

        // -----------------------------------------------------------------------------
        public void HandleMessage(byte[] messageBody)
        {
            try
            {
                // Build native AD event including AD object from byte array
                GK.AD.DTO.ADEvent adEvent = GK.AppCore.Utility.Serializer.DeSerializeFromByteArray<GK.AD.DTO.ADEvent>(messageBody);

                var handler = _eventhandlerFactory.CreateEventHandler(adEvent);
                if (handler != null)
                {
                    handler.HandleEvent();
                }
            }
            catch (Exception ex)
            {
                throw new GK.AppCore.Error.RetryMessageException("See inner exception for details", ex);
            }
        }

        // -----------------------------------------------------------------------------
        public void HandleMessage(object messageBody) { throw new NotImplementedException(); }

    }
}
