using GK.AD;
using GK.AppCore.Utility;
using System;

namespace ADEventSatellite.Model
{
    // ================================================================================
    public class ExceptionHandlerChangeNotifierAgent : IChangeNotifierAgent
    {
        readonly IChangeNotifierAgent _innerAgent;

        // -----------------------------------------------------------------------------
        public ExceptionHandlerChangeNotifierAgent(IChangeNotifierAgent agent)
        {
            _innerAgent = Verify.ArgumentNotNull(agent, "agent");
        }

        // -----------------------------------------------------------------------------
        public ChangeNotifier NewChangeNotifier(EventHandler<ADObjectChangedEventArgs> handler)
        {
            try
            {
                return _innerAgent.NewChangeNotifier(handler);
            }
            catch
            {
                throw;
            }
        }

        // -----------------------------------------------------------------------------
        public void DisposeChangeNotifier(ref ChangeNotifier cn)
        {
            try
            {
                _innerAgent.DisposeChangeNotifier(ref cn);
            }
            catch
            {
                throw;
            }
        }
    }
}